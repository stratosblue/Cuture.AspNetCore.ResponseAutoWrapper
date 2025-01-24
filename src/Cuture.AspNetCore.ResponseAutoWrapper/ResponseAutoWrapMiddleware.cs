using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 响应自动包装中间件
/// </summary>
internal class ResponseAutoWrapMiddleware
{
    #region Private 字段

    private readonly Func<HttpContext, Exception, object?> _exceptionWrapDelegate;

    private readonly ILogger _logger;

    private readonly RequestDelegate _next;

    private readonly bool _notCatchExceptions;

    private readonly Func<HttpContext, object?> _notOKStatusCodeWrapDelegate;

    /// <inheritdoc cref="ResponseAutoWrapMiddlewareOptions.ThrowCaughtExceptions"/>
    private readonly bool _throwCaughtExceptions;

    #region OutputFormat

    private readonly IOutputFormatter _defaultOutputFormatter;

    private readonly IHttpResponseStreamWriterFactory _httpResponseStreamWriterFactory;

    /// <inheritdoc cref="ResponseAutoWrapMiddlewareOptions.IgnoreOptionsRequest"/>
    private readonly bool _ignoreOptionsRequest;

    private readonly OutputFormatterSelector _outputFormatterSelector;

    #endregion OutputFormat

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="ResponseAutoWrapMiddleware"/>
    public ResponseAutoWrapMiddleware(RequestDelegate next,
                                      IServiceProvider serviceProvider,
                                      ResponseAutoWrapMiddlewareOptions options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));

        if (serviceProvider is null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (options.DefaultOutputFormatterSelector is null)
        {
            throw new InvalidOperationException($"{nameof(options)}.{nameof(options.DefaultOutputFormatterSelector)} can not be null.");
        }

        #region OutputFormat

        _defaultOutputFormatter = options.DefaultOutputFormatterSelector(GetService<IOptions<MvcOptions>>().Value.OutputFormatters)
                                  ?? throw new InvalidOperationException($"{nameof(options.DefaultOutputFormatterSelector)} returned null. There must have a default OutputFormatter.");

        _outputFormatterSelector = GetService<OutputFormatterSelector>();

        _httpResponseStreamWriterFactory = GetService<IHttpResponseStreamWriterFactory>();

        #endregion OutputFormat

        _logger = GetService<ILoggerFactory>().CreateLogger($"Cuture.AspNetCore.ResponseAutoWrapper.{nameof(ResponseAutoWrapMiddleware)}");

        _notCatchExceptions = !options.CatchExceptions;
        _throwCaughtExceptions = options.ThrowCaughtExceptions;
        _ignoreOptionsRequest = options.IgnoreOptionsRequest;

        var delegateCollection = GetService<ResponseAutoWrapperWorkDelegateCollection>();

        _exceptionWrapDelegate = delegateCollection.ExceptionWrapDelegate;
        _notOKStatusCodeWrapDelegate = delegateCollection.NotOKStatusCodeWrapDelegate;

        TService GetService<TService>() where TService : notnull => serviceProvider.GetRequiredService<TService>();
    }

    #endregion Public 构造函数

    #region Public 方法

    public async Task InvokeAsync(HttpContext context)
    {
        if (_ignoreOptionsRequest
            && HttpMethods.IsOptions(context.Request.Method))
        {
            await _next(context);
            return;
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (_notCatchExceptions)
            {
                throw;
            }

            //响应未开始，则包装响应
            if (!context.Response.HasStarted
                && !context.RequestAborted.IsCancellationRequested)
            {
                var response = _exceptionWrapDelegate(context, ex);
                if (response is not null)
                {
                    await WriteResponseWithFormatterAsync(context, response);
                }
            }
            else //无法对响应进行包装，此时强制向上层抛出异常
            {
                throw;
            }

            if (_throwCaughtExceptions)
            {
                throw;
            }
            else //不抛出异常时，触发回调
            {
                LogMiddlewareException(context.Request, ex);
            }
        }
        finally
        {
            if (!context.Response.HasStarted
                && !context.RequestAborted.IsCancellationRequested
                && context.Response.StatusCode != StatusCodes.Status200OK)
            {
                var response = _notOKStatusCodeWrapDelegate(context);
                if (response is not null)
                {
                    context.Response.Headers.ContentLength = null;
                    await WriteResponseWithFormatterAsync(context, response);
                }
            }
        }
    }

    #endregion Public 方法

    #region Private 方法

    /// <summary>
    /// 记录中间件异常
    /// </summary>
    /// <param name="request">出现异常的请求</param>
    /// <param name="exception">异常</param>
    private void LogMiddlewareException(HttpRequest request, Exception exception)
    {
        //https://github.com/dotnet/aspnetcore/tree/8dd33378697e6f8ca89116170ec3046c185724b6/src/Hosting/Hosting/src/Internal/HostingRequestStartingLog.cs
        _logger.LogError(exception, "Request error {Protocol} {Method} {Scheme}://{Host}{PathBase}{Path}{QueryString} {ContentType} {ContentLength}",
                         request.Protocol,
                         request.Method,
                         request.Scheme,
                         request.Host.Value,
                         request.PathBase.Value,
                         request.Path.Value,
                         request.QueryString.Value,
                         request.ContentType ?? string.Empty,
                         request.ContentLength.HasValue ? request.ContentLength.ToString() : string.Empty);
    }

    #endregion Private 方法

    #region Internal

    private static MediaTypeCollection? CreateMediaTypeCollection(IList<MediaTypeHeaderValue> parsedAcceptValues)
    {
        if (parsedAcceptValues.Count == 0)
        {
            return null;
        }
        var result = new MediaTypeCollection();
        foreach (var mediaType in parsedAcceptValues)
        {
            result.Add(mediaType);
        }
        return result;
    }

    private Task WriteResponseWithFormatterAsync(HttpContext context, object response)
    {
        var formatterWriteContext = new OutputFormatterWriteContext(context,
                                                                    _httpResponseStreamWriterFactory.CreateWriter,
                                                                    response.GetType(),
                                                                    response);

        if (context.Request.Headers.TryGetValue(HeaderNames.Accept, out var acceptValues)
            && acceptValues.Count > 0
            && !EmptyOrHasWildcard(acceptValues))
        {
            return WriteResponseWithSelectFormatterAsync(context, formatterWriteContext);
        }

        // 有通配符，直接使用默认Formatter
        return _defaultOutputFormatter.WriteAsync(formatterWriteContext);

        static bool EmptyOrHasWildcard(in StringValues acceptValues)
        {
            // 检查通配符
            return acceptValues.Count switch
            {
                1 => NullOrHasWildcard(acceptValues, 0),
                2 => NullOrHasWildcard(acceptValues, 0) && NullOrHasWildcard(acceptValues, 1),
                { } count => HasAnyWildcard(acceptValues, count),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool HasAnyWildcard(in StringValues acceptValues, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (NullOrHasWildcard(acceptValues, i))
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool NullOrHasWildcard(in StringValues acceptValues, in int index)
        {
            var value = acceptValues[index];
            return value is null || value.Contains("*/*", StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// 选择Formatter进行响应
    /// </summary>
    /// <param name="context"></param>
    /// <param name="formatterWriteContext"></param>
    /// <returns></returns>
    private Task WriteResponseWithSelectFormatterAsync(HttpContext context, OutputFormatterWriteContext formatterWriteContext)
    {
        var accepts = context.Request.Headers.TryGetValue(HeaderNames.Accept, out var acceptValue)
                      ? MediaTypeHeaderValue.TryParseList(acceptValue, out var parsedAcceptValues)
                        ? CreateMediaTypeCollection(parsedAcceptValues)
                        : null
                      : null;

        var selectedFormatter = accepts is null
                                ? _defaultOutputFormatter
                                : _outputFormatterSelector.SelectFormatter(formatterWriteContext,
                                                                           Array.Empty<IOutputFormatter>(),
                                                                           accepts)
                                  ?? _defaultOutputFormatter;

        return selectedFormatter.WriteAsync(formatterWriteContext);
    }

    #endregion Internal
}
