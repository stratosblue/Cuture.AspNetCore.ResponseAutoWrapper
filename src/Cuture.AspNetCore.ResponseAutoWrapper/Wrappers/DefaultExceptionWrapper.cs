using System;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

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
    private readonly int? _rewriteStatusCode;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="DefaultExceptionWrapper{TResponse}"/>
    public DefaultExceptionWrapper(IResponseCreator<TResponse> responseCreator,
                                   IExceptionMessageProvider exceptionMessageProvider,
                                   IOptions<ResponseAutoWrapperOptions> optionsAccessor)
    {
        _responseCreator = responseCreator ?? throw new ArgumentNullException(nameof(responseCreator));
        _exceptionMessageProvider = exceptionMessageProvider ?? throw new ArgumentNullException(nameof(exceptionMessageProvider));
        _rewriteStatusCode = optionsAccessor?.Value?.RewriteStatusCode;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public TResponse? Wrap(HttpContext httpContext, Exception exception)
    {
        if (httpContext.IsSetDoNotWrap())
        {
            return null;
        }

        if (_rewriteStatusCode.HasValue)
        {
            httpContext.Response.StatusCode = _rewriteStatusCode.Value;
        }

        return _responseCreator.Create(code: StatusCodes.Status500InternalServerError,
                                       message: _exceptionMessageProvider.ParseMessage(httpContext, exception));
    }

    #endregion Public 方法
}