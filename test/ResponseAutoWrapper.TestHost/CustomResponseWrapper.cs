using System;
using System.Diagnostics;
using System.Net;

using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace ResponseAutoWrapper.TestHost;

public class NotGenericCustomResponseWrapper : AbstractResponseWrapper<CustomResponse, ResponseCode, ResponseMessage>
{
    #region Public 构造函数

    public NotGenericCustomResponseWrapper(IWrapTypeCreator<ResponseCode, ResponseMessage> wrapTypeCreator, IOptions<ResponseAutoWrapperOptions> optionsAccessor)
        : base(wrapTypeCreator, optionsAccessor)
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    public override CustomResponse? ExceptionWrap(HttpContext context, Exception exception) => new() { Code = new(ResponseState.Error, 30000), Message = new() { Text = exception.Message, TraceId = Activity.Current.TraceId } };

    public override CustomResponse? InvalidModelStateWrap(ActionContext context) => new() { Code = new(ResponseState.Error, 20000) };

    public override CustomResponse? NotOKStatusCodeWrap(HttpContext context)
    {
        var statusCode = context.Response.StatusCode;
        if (statusCode is >= 300 and < 400)
        {
            return null;
        }

        ResponseCode code;
        ResponseMessage message;

        if (context.TryGetResponseDescription<ResponseCode, ResponseMessage>(out var description))
        {
            code = description.Code;
            message = description.Message;
        }
        else
        {
            code = new(ResponseState.Error, 25000);
            message = new() { Text = Enum.IsDefined(typeof(HttpStatusCode), statusCode) ? ((HttpStatusCode)statusCode).ToString() : null };
        }

        return new() { Code = code, Message = message };
    }

    #endregion Public 方法

    #region Protected 方法

    protected override CustomResponse? ActionEmptyResultWrap(ResultExecutingContext context, EmptyResult emptyResult, ResponseDescription<ResponseCode, ResponseMessage>? description)
    {
        return new() { Code = description?.Code ?? ResponseCode.Success, Message = description?.Message };
    }

    protected override CustomResponse? ActionObjectResultWrap(ResultExecutingContext context, ObjectResult objectResult, ResponseDescription<ResponseCode, ResponseMessage>? description)
    {
        return new() { Code = description?.Code ?? ResponseCode.Success, Message = description?.Message, Data = objectResult.Value };
    }

    #endregion Protected 方法
}

public class CustomResponseWrapper : AbstractResponseWrapper<CustomResponse<object>, ResponseCode, ResponseMessage>
{
    #region Public 构造函数

    public CustomResponseWrapper(IWrapTypeCreator<ResponseCode, ResponseMessage> wrapTypeCreator, IOptions<ResponseAutoWrapperOptions> optionsAccessor)
        : base(wrapTypeCreator, optionsAccessor)
    {
    }

    #endregion Public 构造函数

    #region Public 方法

    public override CustomResponse<object>? ExceptionWrap(HttpContext context, Exception exception) => new() { Code = new(ResponseState.Error, 30000), Message = new() { Text = exception.Message, TraceId = Activity.Current.TraceId } };

    public override CustomResponse<object>? InvalidModelStateWrap(ActionContext context) => new() { Code = new(ResponseState.Error, 20000) };

    public override CustomResponse<object>? NotOKStatusCodeWrap(HttpContext context)
    {
        var statusCode = context.Response.StatusCode;
        if (statusCode is >= 300 and < 400)
        {
            return null;
        }

        ResponseCode code;
        ResponseMessage message;

        if (context.TryGetResponseDescription<ResponseCode, ResponseMessage>(out var description))
        {
            code = description.Code;
            message = description.Message;
        }
        else
        {
            code = new(ResponseState.Error, 25000);
            message = new() { Text = Enum.IsDefined(typeof(HttpStatusCode), statusCode) ? ((HttpStatusCode)statusCode).ToString() : null };
        }

        return new() { Code = code, Message = message };
    }

    #endregion Public 方法

    #region Protected 方法

    protected override CustomResponse<object>? ActionEmptyResultWrap(ResultExecutingContext context, EmptyResult emptyResult, ResponseDescription<ResponseCode, ResponseMessage>? description)
    {
        return new() { Code = description?.Code ?? ResponseCode.Success, Message = description?.Message };
    }

    protected override CustomResponse<object>? ActionObjectResultWrap(ResultExecutingContext context, ObjectResult objectResult, ResponseDescription<ResponseCode, ResponseMessage>? description)
    {
        return new() { Code = description?.Code ?? ResponseCode.Success, Message = description?.Message, Data = objectResult.Value };
    }

    #endregion Protected 方法
}
