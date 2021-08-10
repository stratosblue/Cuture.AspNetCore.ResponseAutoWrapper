using System;

using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 默认的 <inheritdoc cref="IExceptionWrapper{TResponse}"/>
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public sealed class DefaultExceptionWrapper<TResponse> : IExceptionWrapper<TResponse>
        where TResponse : class
    {
        #region Private 字段

        private readonly IExceptionMessageProvider _exceptionMessageProvider;
        private readonly IResponseCreator<TResponse> _responseCreator;

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="DefaultExceptionWrapper{TResponse}"/>
        public DefaultExceptionWrapper(IResponseCreator<TResponse> responseCreator,
                                       IExceptionMessageProvider exceptionMessageProvider)
        {
            _responseCreator = responseCreator ?? throw new ArgumentNullException(nameof(responseCreator));
            _exceptionMessageProvider = exceptionMessageProvider ?? throw new ArgumentNullException(nameof(exceptionMessageProvider));
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public TResponse? Wrap(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;

            return _responseCreator.Create(code: StatusCodes.Status500InternalServerError,
                                           message: _exceptionMessageProvider.ParseMessage(httpContext, exception));
        }

        #endregion Public 方法
    }
}