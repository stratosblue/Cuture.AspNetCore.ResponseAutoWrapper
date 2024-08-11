using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 抽象响应包装器
/// </summary>
/// <typeparam name="TResponse">统一响应类型</typeparam>
/// <typeparam name="TCode">Code类型</typeparam>
/// <typeparam name="TMessage">Message类型</typeparam>
public abstract class AbstractResponseWrapper<TResponse, TCode, TMessage>
    : IActionResultWrapper<TResponse, TCode, TMessage>
    , INotOKStatusCodeWrapper<TResponse, TCode, TMessage>
    , IExceptionWrapper<TResponse, TCode, TMessage>
    , IInvalidModelStateWrapper<TResponse, TCode, TMessage>
    where TResponse : class
{
    #region Private 字段

    /// <inheritdoc cref="ResponseAutoWrapperOptions.RewriteStatusCode"/>
    protected readonly int? RewriteStatusCode;

    /// <inheritdoc cref="IWrapTypeCreator{TCode, TMessage}"/>
    protected readonly IWrapTypeCreator<TCode, TMessage> WrapTypeCreator;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="AbstractResponseWrapper{TResponse, TCode, TMessage}"/>
    public AbstractResponseWrapper(IWrapTypeCreator<TCode, TMessage> wrapTypeCreator, IOptions<ResponseAutoWrapperOptions> optionsAccessor)
    {
        WrapTypeCreator = wrapTypeCreator ?? throw new ArgumentNullException(nameof(wrapTypeCreator));
        RewriteStatusCode = optionsAccessor?.Value?.RewriteStatusCode;
    }

    #endregion Public 构造函数

    #region Wrap

    #region ActionResultWrap

    /// <summary>
    /// <inheritdoc cref="IActionResultWrapper{TResponse, TCode, TMessage}"/><para/>
    /// <inheritdoc cref="IActionResultWrapper{TResponse, TCode, TMessage}.Wrap(ResultExecutingContext)"/><para/>
    /// 派生于 <see cref="IActionResultWrapper{TResponse, TCode, TMessage}.Wrap(ResultExecutingContext)"/><para/>
    /// 仅当 <paramref name="context"/> 返回值类型为 <see cref="EmptyResult"/> 时执行
    /// </summary>
    /// <param name="context"></param>
    /// <param name="emptyResult"></param>
    /// <param name="description">当前请求的响应描述</param>
    /// <returns></returns>
    protected abstract TResponse? ActionEmptyResultWrap(ResultExecutingContext context, EmptyResult emptyResult, ResponseDescription<TCode, TMessage>? description);

    /// <summary>
    /// <inheritdoc cref="IActionResultWrapper{TResponse, TCode, TMessage}"/><para/>
    /// <inheritdoc cref="IActionResultWrapper{TResponse, TCode, TMessage}.Wrap(ResultExecutingContext)"/><para/>
    /// 派生于 <see cref="IActionResultWrapper{TResponse, TCode, TMessage}.Wrap(ResultExecutingContext)"/><para/>
    /// 仅当 <paramref name="context"/> 返回值类型为 <see cref="ObjectResult"/> 时执行
    /// </summary>
    /// <param name="context"></param>
    /// <param name="objectResult"></param>
    /// <param name="description">当前请求的响应描述</param>
    /// <returns></returns>
    protected abstract TResponse? ActionObjectResultWrap(ResultExecutingContext context, ObjectResult objectResult, ResponseDescription<TCode, TMessage>? description);

    #endregion ActionResultWrap

    /// <summary>
    /// <inheritdoc cref="IExceptionWrapper{TResponse, TCode, TMessage}"/><para/>
    /// <inheritdoc cref="IExceptionWrapper{TResponse, TCode, TMessage}.Wrap(HttpContext, Exception)"/><para/>
    /// 等价于 <see cref="IExceptionWrapper{TResponse, TCode, TMessage}.Wrap(HttpContext, Exception)"/>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <returns></returns>
    public abstract TResponse? ExceptionWrap(HttpContext context, Exception exception);

    /// <summary>
    /// <inheritdoc cref="IInvalidModelStateWrapper{TResponse, TCode, TMessage}"/><para/>
    /// <inheritdoc cref="IInvalidModelStateWrapper{TResponse, TCode, TMessage}.Wrap(ActionContext)"/><para/>
    /// 等价于 <see cref="IInvalidModelStateWrapper{TResponse, TCode, TMessage}.Wrap(ActionContext)"/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public abstract TResponse? InvalidModelStateWrap(ActionContext context);

    /// <summary>
    /// <inheritdoc cref="INotOKStatusCodeWrapper{TResponse, TCode, TMessage}"/><para/>
    /// <inheritdoc cref="INotOKStatusCodeWrapper{TResponse, TCode, TMessage}.Wrap(HttpContext)"/><para/>
    /// 等价于 <see cref="INotOKStatusCodeWrapper{TResponse, TCode, TMessage}.Wrap(HttpContext)"/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public abstract TResponse? NotOKStatusCodeWrap(HttpContext context);

    #endregion Wrap

    #region Protected 方法

    /// <summary>
    /// <inheritdoc cref="IActionResultWrapper{TResponse, TCode, TMessage}"/><para/>
    /// <inheritdoc cref="IActionResultWrapper{TResponse, TCode, TMessage}.Wrap(ResultExecutingContext)"/><para/>
    /// 等价于 <see cref="IActionResultWrapper{TResponse, TCode, TMessage}.Wrap(ResultExecutingContext)"/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected virtual TResponse? ActionResultWrap(ResultExecutingContext context)
    {
        return context.Result switch
        {
            ObjectResult objectResult => InternalActionObjectResultWrap(context, objectResult),
            EmptyResult emptyResult => ActionEmptyResultWrap(context, emptyResult, context.GetResponseDescription<TCode, TMessage>()),
            _ => null,
        };
    }

    #endregion Protected 方法

    #region Private 方法

    private TResponse? InternalActionObjectResultWrap(ResultExecutingContext context, ObjectResult objectResult)
    {
        if (objectResult.Value is not TResponse
            && (objectResult.Value is null
                || WrapTypeCreator.ShouldWrap(objectResult.Value.GetType())))
        {
            return ActionObjectResultWrap(context, objectResult, context.GetResponseDescription<TCode, TMessage>());
        }
        return null;
    }

    #endregion Private 方法

    #region InterfaceImpl

    TResponse? IInvalidModelStateWrapper<TResponse, TCode, TMessage>.Wrap(ActionContext context)
    {
        if (ShouldSkipWrap(context.HttpContext))
        {
            return null;
        }
        return AfterWrap(context.HttpContext, InvalidModelStateWrap(context));
    }

    TResponse? IActionResultWrapper<TResponse, TCode, TMessage>.Wrap(ResultExecutingContext context)
    {
        if (ShouldSkipWrap(context.HttpContext))
        {
            return null;
        }
        return AfterWrap(context.HttpContext, ActionResultWrap(context));
    }

    TResponse? INotOKStatusCodeWrapper<TResponse, TCode, TMessage>.Wrap(HttpContext context)
    {
        if (ShouldSkipWrap(context))
        {
            return null;
        }
        return AfterWrap(context, NotOKStatusCodeWrap(context));
    }

    TResponse? IExceptionWrapper<TResponse, TCode, TMessage>.Wrap(HttpContext context, Exception exception)
    {
        if (ShouldSkipWrap(context))
        {
            return null;
        }

        return AfterWrap(context, ExceptionWrap(context, exception));
    }

    #endregion InterfaceImpl

    #region Protected 方法

    /// <summary>
    /// 在包装后执行的代码
    /// </summary>
    /// <param name="httpContext">Http上下文</param>
    /// <param name="wrappedResponse">包装后的响应值</param>
    protected virtual TResponse? AfterWrap(HttpContext httpContext, TResponse? wrappedResponse)
    {
        if (wrappedResponse is not null
            && RewriteStatusCode.HasValue)
        {
            httpContext.Response.StatusCode = RewriteStatusCode.Value;
        }
        return wrappedResponse;
    }

    /// <summary>
    /// 检查是否应该跳过包装
    /// </summary>
    /// <param name="httpContext">Http上下文</param>
    /// <returns>是否应该跳过包装</returns>
    protected virtual bool ShouldSkipWrap(HttpContext httpContext) => httpContext.IsSetDoNotWrapResponse();

    #endregion Protected 方法
}
