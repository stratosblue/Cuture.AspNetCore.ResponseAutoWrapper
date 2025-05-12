using System;

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// API响应
/// </summary>
[Serializable]
public class ApiResponse(int code) : ApiResponse<object>(code)
{

    #region Public 方法

    /// <summary>
    /// 创建一个响应
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="code"></param>
    /// <returns></returns>
    public static ApiResponse<TData> Create<TData>(int code) => new() { Code = code };

    /// <summary>
    /// 创建一个响应
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ApiResponse<TData> Create<TData>(TData? data) => new() { Data = data };

    /// <summary>
    /// 创建一个响应
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="code"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static ApiResponse<TData> Create<TData>(int code, TData? data) => new() { Code = code, Data = data };

    /// <summary>
    /// 创建一个响应
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ApiResponse<TData> Create<TData>(int code, string message) => new() { Code = code, Message = message };

    /// <summary>
    /// 创建一个响应
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="code"></param>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ApiResponse<TData> Create<TData>(int code, TData? data, string? message) => new() { Code = code, Data = data, Message = message };

    #endregion Public 方法
}

/// <summary>
/// 有数据的API响应
/// </summary>
/// <typeparam name="TData"></typeparam>
[Serializable]
public class ApiResponse<TData>(int code) : GenericApiResponse<int, string, TData>(code)
{
    #region Public 构造函数

    /// <inheritdoc cref="ApiResponse"/>
    public ApiResponse() : this(StatusCodes.Status200OK)
    {
    }

    #endregion Public 构造函数
}
