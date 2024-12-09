﻿using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 无效模型状态包装器
/// </summary>
/// <typeparam name="TResponse">统一响应类型</typeparam>
/// <typeparam name="TCode">Code类型</typeparam>
/// <typeparam name="TMessage">Message类型</typeparam>
public interface IInvalidModelStateWrapper<TResponse, TCode, TMessage> : IWrapper<TResponse, TCode, TMessage>
{
    #region Public 方法

    /// <summary>
    /// 通过 <see cref="ActionContext"/> 返回包装后的统一响应
    /// </summary>
    /// <param name="context"></param>
    /// <returns>包装后的统一响应类型对象，返回 null ，则使用原始处理逻辑进行处理</returns>
    TResponse? Wrap(ActionContext context);

    #endregion Public 方法
}
