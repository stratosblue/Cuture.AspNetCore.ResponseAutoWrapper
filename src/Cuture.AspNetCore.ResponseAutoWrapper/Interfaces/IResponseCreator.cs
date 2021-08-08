using System;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 响应创建器
    /// </summary>
    public interface IResponseCreator
    {
        #region Public 方法

        /// <summary>
        /// 创建响应对象
        /// </summary>
        /// <param name="code">响应code</param>
        /// <param name="message">响应消息</param>
        /// <param name="exception">处理过程中已触发的异常</param>
        /// <returns></returns>
        object CreateObject(int code, string? message = null, Exception? exception = null);

        #endregion Public 方法
    }

    /// <summary>
    /// 有类型的响应创建器
    /// </summary>
    /// <typeparam name="TResponse">响应类型</typeparam>
    public interface IResponseCreator<TResponse> : IResponseCreator
        where TResponse : notnull
    {
        #region Public 方法

        /// <inheritdoc cref="IResponseCreator.CreateObject(int, string?, Exception?)"/>
        TResponse Create(int code, string? message = null);

        /// <summary>
        /// 创建响应对象
        /// </summary>
        /// <param name="code">响应code</param>
        /// <param name="data">响应数据</param>
        /// <param name="message">响应消息</param>
        /// <returns></returns>
        TResponse Create(int code, object? data, string? message = null);

        /// <inheritdoc cref="IResponseCreator.CreateObject(int, string?, Exception?)"/>
        TResponse Create(int code, Exception exception, string? message = null);

        #endregion Public 方法
    }
}