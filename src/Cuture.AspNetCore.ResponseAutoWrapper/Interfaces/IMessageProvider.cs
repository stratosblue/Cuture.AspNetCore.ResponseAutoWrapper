using System;

using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 消息提供器<para/>
    /// 在中间件中处理非成功状态的http请求时，用以提供返回消息
    /// </summary>
    public interface IMessageProvider
    {
        #region Public 方法

        /// <inheritdoc cref="GetMessage(int, HttpContext?, Exception?)"/>
        string GetMessage(int code);

        /// <inheritdoc cref="GetMessage(int, HttpContext?, Exception?)"/>
        string GetMessage(int code, HttpContext? context);

        /// <summary>
        /// 获取要消息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        string GetMessage(int code, HttpContext? context, Exception? exception);

        #endregion Public 方法
    }
}