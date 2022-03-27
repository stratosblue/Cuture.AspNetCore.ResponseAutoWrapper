using System;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 默认的 <inheritdoc cref="IInvalidModelStateWrapper{TResponse}"/>
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public class DefaultInvalidModelStateWrapper<TResponse> : IInvalidModelStateWrapper<TResponse>
    where TResponse : class
{
    #region Private 字段

    private readonly IResponseCreator<TResponse> _responseCreator;
    private readonly int? _rewriteStatusCode;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="DefaultInvalidModelStateWrapper{TResponse}"/>
    public DefaultInvalidModelStateWrapper(IResponseCreator<TResponse> responseCreator,
                                           IOptions<ResponseAutoWrapperOptions> optionsAccessor)
    {
        _responseCreator = responseCreator ?? throw new ArgumentNullException(nameof(responseCreator));
        _rewriteStatusCode = optionsAccessor?.Value?.RewriteStatusCode;
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public TResponse? Wrap(ActionContext context)
    {
        if (context.HttpContext.IsSetDoNotWrap())
        {
            return null;
        }

        if (_rewriteStatusCode.HasValue)
        {
            context.HttpContext.Response.StatusCode = _rewriteStatusCode.Value;
        }

        var errorMessages = context.ModelState.Where(m => m.Value?.Errors.Count > 0)
                                              .Select(m => $"{m.Key} - {m.Value?.Errors.FirstOrDefault()?.ErrorMessage}");

        var message = string.Join(Environment.NewLine, errorMessages);

        return _responseCreator.Create(code: StatusCodes.Status400BadRequest, message: message);
    }

    #endregion Public 方法
}