using System;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 包装类型创建器<para/>
/// </summary>
public interface IWrapTypeCreator
{
    #region Public 属性

    /// <summary>
    /// 响应类型泛型定义
    /// </summary>
    Type? ResponseGenericType { get; }

    /// <summary>
    /// 响应类型
    /// </summary>
    Type ResponseType { get; }

    #endregion Public 属性

    #region Public 方法

    /// <summary>
    /// 创建包装后的类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    Type MakeWrapType(Type type);

    /// <summary>
    /// 是否应该包装
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool ShouldWrap(Type type);

    #endregion Public 方法
}
