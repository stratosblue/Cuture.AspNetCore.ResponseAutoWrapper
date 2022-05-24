using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 兼容旧响应格式的包装器的选项
/// </summary>
public class LegacyCompatibleResponseWrapperOptions
{
    #region Public 属性

    #region Codes

    /// <summary>
    /// 异常时的Code
    /// </summary>
    public int ExceptionCode { get; set; } = StatusCodes.Status500InternalServerError;

    /// <summary>
    /// 无效模型时的Code
    /// </summary>
    public int InvalidModelStateCode { get; set; } = StatusCodes.Status400BadRequest;

    /// <summary>
    /// 成功时的Code
    /// </summary>
    public int SuccessCode { get; set; } = StatusCodes.Status200OK;

    #endregion Codes

    #region Messages

    /// <summary>
    /// 在 <see cref="IActionResultWrapper{TResponse, TCode, TMessage}.Wrap(ResultExecutingContext)"/> 中使用的默认Message
    /// </summary>
    public string ActionResultWrapMessage { get; set; } = "SUCCESS";

    /// <summary>
    /// 在 <see cref="IExceptionWrapper{TResponse, TCode, TMessage}.Wrap(HttpContext, Exception)"/> 中使用的默认Message
    /// </summary>
    public string ExceptionWrapMessage { get; set; } = "Server Error";

    #endregion Messages

    #endregion Public 属性
}
