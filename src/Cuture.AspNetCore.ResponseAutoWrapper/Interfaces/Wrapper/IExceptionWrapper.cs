﻿using System;

using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 异常包装器
/// </summary>
/// <typeparam name="TResponse">统一响应类型</typeparam>
/// <typeparam name="TCode">Code类型</typeparam>
/// <typeparam name="TMessage">Message类型</typeparam>
public interface IExceptionWrapper<TResponse, TCode, TMessage> : IWrapper<TResponse, TCode, TMessage>
{
    #region Public 方法

    /// <summary>
    /// 通过 <see cref="HttpContext"/> 和 <see cref="Exception"/> 返回包装后的统一响应
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    /// <returns>包装后的统一响应类型对象，返回 null 时，不做处理</returns>
    TResponse? Wrap(HttpContext context, Exception exception);

    #endregion Public 方法
}
