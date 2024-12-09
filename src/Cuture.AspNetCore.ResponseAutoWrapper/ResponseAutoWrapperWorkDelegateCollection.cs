using System;

using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 响应自动包装工作委托集合
/// </summary>
public class ResponseAutoWrapperWorkDelegateCollection
{
    #region Public 属性

    /// <summary>
    /// 异常包装委托
    /// </summary>
    public Func<HttpContext, Exception, object?> ExceptionWrapDelegate { get; }

    /// <summary>
    /// 非200状态码包装委托
    /// </summary>
    public Func<HttpContext, object?> NotOKStatusCodeWrapDelegate { get; }

    #endregion Public 属性

    #region Public 构造函数

    /// <summary>
    /// <inheritdoc cref="ResponseAutoWrapperWorkDelegateCollection"/>
    /// </summary>
    /// <param name="exceptionWrapDelegate">异常包装委托</param>
    /// <param name="notOKStatusCodeWrapDelegate">非200状态码包装委托</param>
    public ResponseAutoWrapperWorkDelegateCollection(Func<HttpContext, Exception, object?> exceptionWrapDelegate,
                                                     Func<HttpContext, object?> notOKStatusCodeWrapDelegate)
    {
        ExceptionWrapDelegate = exceptionWrapDelegate ?? throw new ArgumentNullException(nameof(exceptionWrapDelegate));
        NotOKStatusCodeWrapDelegate = notOKStatusCodeWrapDelegate ?? throw new ArgumentNullException(nameof(notOKStatusCodeWrapDelegate));
    }

    #endregion Public 构造函数
}
