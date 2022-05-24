using System;
using System.Diagnostics;

using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace CustomStructureWebApplication;

public class CustomWrapper : AbstractResponseWrapper<CommonResponse<object>, string, RichMessage>
{
    #region Public 构造函数

    public CustomWrapper(IWrapTypeCreator<string, RichMessage> wrapTypeCreator, IOptions<ResponseAutoWrapperOptions> optionsAccessor) : base(wrapTypeCreator, optionsAccessor)
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    public override CommonResponse<object>? ExceptionWrap(HttpContext context, Exception exception)
    {
        return new CommonResponse<object>() { Code = "E4000", Message = new RichMessage() { Content = "SERVER ERROR" }, TraceId = Activity.Current.TraceId.ToString() };
    }

    public override CommonResponse<object>? InvalidModelStateWrap(ActionContext context)
    {
        return new CommonResponse<object>() { Code = "E3000", Message = new RichMessage() { Content = "INPUT ERROR" }, TraceId = Activity.Current.TraceId.ToString() };
    }

    public override CommonResponse<object>? NotOKStatusCodeWrap(HttpContext context)
    {
        return null;
    }

    #endregion Public 方法

    #region Protected 方法

    protected override CommonResponse<object>? ActionEmptyResultWrap(ResultExecutingContext context, EmptyResult emptyResult, ResponseDescription<string, RichMessage>? description)
    {
        return new CommonResponse<object>() { Code = description?.Code ?? "E2000", Message = description?.Message ?? new RichMessage() { Content = "NO CONTENT" } };
    }

    protected override CommonResponse<object>? ActionObjectResultWrap(ResultExecutingContext context, ObjectResult objectResult, ResponseDescription<string, RichMessage>? description)
    {
        return new CommonResponse<object>() { Code = description?.Code ?? "E1000", Data = objectResult.Value, Message = description?.Message ?? new RichMessage() { Content = "OK" } };
    }

    #endregion Protected 方法
}
