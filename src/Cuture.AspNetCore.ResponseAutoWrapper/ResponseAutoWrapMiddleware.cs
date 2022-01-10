using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
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

        private readonly IResponseDirectWriter _responseDirectWriter;

        /// <inheritdoc cref="ResponseAutoWrapMiddlewareOptions.ThrowCaughtExceptions"/>
        private readonly bool _throwCaughtExceptions;

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

            _responseDirectWriter = GetService<IResponseDirectWriter>();
            _logger = GetService<ILogger<ResponseAutoWrapMiddleware>>();

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
                        await _responseDirectWriter.WriteAsync(context, response);
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
                        await _responseDirectWriter.WriteAsync(context, response);
                    }
                }
            }
        }

        #endregion Public 方法
    }
}