using System;
using System.Runtime.CompilerServices;

using Cuture.AspNetCore.ResponseAutoWrapper;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 通用的API响应类型
/// </summary>
[Serializable]
public abstract class GenericApiResponse
{
    #region Public 方法

    /// <summary>
    /// 创建一个响应
    /// </summary>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="code"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GenericApiResponse<TCode, TMessage, TData> Create<TCode, TMessage, TData>(TCode code) => new(code);

    /// <summary>
    /// 创建一个响应
    /// </summary>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GenericApiResponse<TCode, TMessage, TData> Create<TCode, TMessage, TData>(TCode code, TMessage? message) => new(code) { Message = message };

    /// <summary>
    /// 创建一个响应
    /// </summary>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="code"></param>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GenericApiResponse<TCode, TMessage, TData> Create<TCode, TMessage, TData>(TCode code, TMessage? message, TData? data) => new(code) { Message = message, Data = data };

    #endregion Public 方法
}

/// <summary>
/// 通用的API响应类型
/// </summary>
/// <typeparam name="TCode">指定Code类型</typeparam>
[Serializable]
public abstract class GenericApiResponse<TCode> : GenericApiResponse
{
    #region Public 属性

    /// <summary>
    /// 状态码
    /// </summary>
    public TCode Code { get; set; }

    #endregion Public 属性

    #region Protected 构造函数

    /// <inheritdoc cref="GenericApiResponse"/>
    protected GenericApiResponse(TCode code)
    {
        Code = code;
    }

    #endregion Protected 构造函数
}

/// <summary>
/// 通用的API响应类型
/// </summary>
/// <typeparam name="TCode">指定Code类型</typeparam>
/// <typeparam name="TMessage">指定Message类型</typeparam>
[Serializable]
public abstract class GenericApiResponse<TCode, TMessage> : GenericApiResponse<TCode>
{
    #region Public 属性

    /// <summary>
    /// 消息
    /// </summary>
    public TMessage? Message { get; set; }

    #endregion Public 属性

    #region Protected 构造函数

    /// <inheritdoc cref="GenericApiResponse"/>
    protected GenericApiResponse(TCode code) : base(code)
    {
    }

    #endregion Protected 构造函数
}

/// <summary>
/// 有数据的通用的API响应类型
/// </summary>
/// <typeparam name="TCode"></typeparam>
/// <typeparam name="TMessage"></typeparam>
/// <typeparam name="TData"></typeparam>
[Serializable]
public class GenericApiResponse<TCode, TMessage, [ResponseData] TData> : GenericApiResponse<TCode, TMessage>
{
    #region Public 属性

    /// <summary>
    /// 数据
    /// </summary>
    public TData? Data { get; set; }

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="GenericApiResponse"/>
    public GenericApiResponse(TCode code) : base(code)
    {
    }

    #endregion Public 构造函数
}
