using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        var delegateCollection = GetService<ResponseAutoWrapperWorkDelegateCollection>();

        _exceptionWrapDelegate = delegateCollection.ExceptionWrapDelegate;
        _notOKStatusCodeWrapDelegate = delegateCollection.NotOKStatusCodeWrapDelegate;

        TService GetService<TService>() where TService : notnull => serviceProvider.GetRequiredService<TService>();
    }

    #endregion Public 构造函数

    #region Public 方法

    public async Task InvokeAsync(HttpContext context)
    {
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

            if (!_throwCaughtExceptions)
            {
                //不抛出异常时，记录异常
                var request = context.Request;
                //https://github.com/dotnet/aspnetcore/tree/8dd33378697e6f8ca89116170ec3046c185724b6/src/Hosting/Hosting/src/Internal/HostingRequestStartingLog.cs
                _logger.LogError(ex, "Request error {Protocol} {Method} {Scheme}://{Host}{PathBase}{Path}{QueryString} {ContentType} {ContentLength}",
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

            //响应未开始，则包装响应
            if (!context.Response.HasStarted)
            {
                var response = _exceptionWrapDelegate(context, ex);
                if (response is not null)
                {
                    await WriteResponseWithFormatterAsync(context, response);
                }
            }

            if (_throwCaughtExceptions)
            {
                throw;
            }
        }
        finally
        {
            if (!context.Response.HasStarted
                && context.Response.StatusCode != 200)
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
        var formatterContext = new OutputFormatterWriteContext(context,
                                                               _httpResponseStreamWriterFactory.CreateWriter,
                                                               response.GetType(),
                                                               response);

        var accepts = context.Request.Headers.TryGetValue(HeaderNames.Accept, out var acceptValue)
                      ? MediaTypeHeaderValue.TryParseList(acceptValue, out var parsedAcceptValues)
                        ? CreateMediaTypeCollection(parsedAcceptValues)
                        : null
                      : null;

        var selectedFormatter = accepts is null
                                ? _defaultOutputFormatter
                                : _outputFormatterSelector.SelectFormatter(formatterContext,
                                                                         Array.Empty<IOutputFormatter>(),
                                                                         accepts)
                                    ?? _defaultOutputFormatter;

        return selectedFormatter.WriteAsync(formatterContext);
    }

    #endregion Internal
}
