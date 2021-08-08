using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 响应直接写入器<para/>
    /// 用于在某些情况下将响应直接写入<see cref="HttpContext"/>
    /// </summary>
    public interface IResponseDirectWriter
    {
        #region Public 方法

        /// <summary>
        /// 写入响应内容
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task WriteAsync(HttpContext httpContext, object? value);

        #endregion Public 方法
    }
}