using System;
using System.Linq;
using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

using TCode = Int32;
using TMessage = String;

/// <summary>
/// 兼容旧响应包装逻辑的包装器（TCode 为 <see cref="int"/>，TMessage 为 <see cref="string"/> 的响应类型）<para/>
/// </summary>
public abstract class LegacyCompatibleResponseWrapper<TResponse> : AbstractResponseWrapper<TResponse, TCode, TMessage>
    where TResponse : class
{
    #region Private 字段

    private readonly LegacyCompatibleResponseWrapperOptions _options;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="LegacyCompatibleResponseWrapper{TResponse}"/>
    public LegacyCompatibleResponseWrapper(IWrapTypeCreator<TCode, TMessage> wrapTypeCreator,
                                           IOptions<ResponseAutoWrapperOptions> optionsAccessor,
                                           IOptions<LegacyCompatibleResponseWrapperOptions> wrapperOptionsAccessor)
        : base(wrapTypeCreator, optionsAccessor)
    {
        ArgumentNullException.ThrowIfNull(wrapperOptionsAccessor?.Value);

        _options = wrapperOptionsAccessor.Value;
    }

    #endregion Public 构造函数

    #region Wrap

    #region ActionResultWrap

    /// <inheritdoc/>
    protected override TResponse? ActionEmptyResultWrap(ResultExecutingContext context, EmptyResult emptyResult, ResponseDescription<TCode, TMessage>? description)
    {
        return CreateResponse(description?.Code ?? _options.SuccessCode, description?.Message ?? _options.ActionResultWrapMessage);
    }

    /// <inheritdoc/>
    protected override TResponse? ActionObjectResultWrap(ResultExecutingContext context, ObjectResult objectResult, ResponseDescription<TCode, TMessage>? description)
    {
        return CreateResponse(description?.Code ?? _options.SuccessCode, description?.Message ?? _options.ActionResultWrapMessage, objectResult.Value);
    }

    #endregion ActionResultWrap

    /// <inheritdoc/>
    public override TResponse? ExceptionWrap(HttpContext context, Exception exception)
    {
        return CreateResponse(_options.ExceptionCode, _options.ExceptionWrapMessage);
    }

    /// <inheritdoc/>
    public override TResponse? InvalidModelStateWrap(ActionContext context)
    {
        var errorMessages = context.ModelState.Where(m => m.Value?.Errors.Count > 0)
                                              .Select(m => $"{m.Key} - {m.Value?.Errors.FirstOrDefault()?.ErrorMessage}");

        var message = string.Join(Environment.NewLine, errorMessages);

        return CreateResponse(_options.InvalidModelStateCode, message);
    }

    /// <inheritdoc/>
    public override TResponse? NotOKStatusCodeWrap(HttpContext context)
    {
        var statusCode = context.Response.StatusCode;
        if (statusCode is >= 300 and < 400)
        {
            return null;
        }

        string? message;

        if (context.TryGetResponseDescription<TCode, TMessage>(out var description))
        {
            statusCode = description.Code;
            message = description.Message;
        }
        else
        {
            message = Enum.IsDefined(typeof(HttpStatusCode), statusCode) ? ((HttpStatusCode)statusCode).ToString() : null;
        }

        return CreateResponse(statusCode, message);
    }

    #endregion Wrap

    #region CreateResponse

    /// <summary>
    /// 创建响应
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    protected abstract TResponse? CreateResponse(TCode code);

    /// <summary>
    /// 创建响应
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    protected abstract TResponse? CreateResponse(TCode code, TMessage? message);

    /// <summary>
    /// 创建响应
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    protected abstract TResponse? CreateResponse(TCode code, TMessage? message, object? data);

    #endregion CreateResponse
}
