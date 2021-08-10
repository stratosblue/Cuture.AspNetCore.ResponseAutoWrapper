using System;

using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// <see cref="Exception"/> 的消息提供器
    /// </summary>
    public interface IExceptionMessageProvider
    {
        #region Public 方法

        /// <summary>
        /// 解析消息
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        string ParseMessage(HttpContext httpContext, Exception exception);

        #endregion Public 方法
    }
}