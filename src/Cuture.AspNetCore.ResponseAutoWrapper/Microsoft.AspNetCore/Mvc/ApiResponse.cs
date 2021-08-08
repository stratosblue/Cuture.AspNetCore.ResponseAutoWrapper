using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// API响应
    /// </summary>
    public abstract class ApiResponse
    {
        #region Public 属性

        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; } = StatusCodes.Status200OK;

        /// <summary>
        /// 消息
        /// </summary>
        public string? Message { get; set; }

        #endregion Public 属性

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
    public class ApiResponse<TData> : ApiResponse
    {
        #region Public 属性

        /// <summary>
        /// 数据
        /// </summary>
        public TData? Data { get; set; }

        #endregion Public 属性
    }
}