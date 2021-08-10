using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Cuture.AspNetCore.ResponseAutoWrapper;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// 响应描述的HttpContext拓展
    /// </summary>
    public static class ResponseDescriptionHttpContextExtensions
    {
        #region Public 方法

        /// <inheritdoc cref="DescribeResponse(HttpContext, int, string)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DescribeResponse(this HttpContext httpContext, int code) => httpContext.Items[typeof(ResponseDescription)] = new ResponseDescription(code);

        /// <inheritdoc cref="DescribeResponse(HttpContext, int, string)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DescribeResponse(this HttpContext httpContext, string message) => httpContext.Items[typeof(ResponseDescription)] = new ResponseDescription(message);

        /// <summary>
        /// 描述响应<para/>
        /// 通过向 <see cref="HttpContext.Items"/> 添加 <see cref="ResponseDescription"/> 以描述响应
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="code">响应码</param>
        /// <param name="message">响应消息</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DescribeResponse(this HttpContext httpContext, int code, string message) => httpContext.Items[typeof(ResponseDescription)] = new ResponseDescription(code, message);

        /// <summary>
        /// 获取描述响应<para/>
        /// 从 <see cref="HttpContext.Items"/> 中读取 <see cref="ResponseDescription"/>
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns>返回 <see cref="HttpContext.Items"/> 中的 <see cref="ResponseDescription"/>，当不存在时，返回 <see cref="ResponseDescription.Empty"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ResponseDescription GetResponseDescription(this HttpContext httpContext)
        {
            if (httpContext.Items.TryGetValue(typeof(ResponseDescription), out var storedObject)
                && storedObject is ResponseDescription storedDescription)
            {
                return storedDescription;
            }
            return ResponseDescription.Empty;
        }

        /// <summary>
        /// 尝试获取描述响应<para/>
        /// 从 <see cref="HttpContext.Items"/> 中读取 <see cref="ResponseDescription"/>
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="description"></param>
        /// <returns>返回 <see cref="HttpContext.Items"/> 中的 <see cref="ResponseDescription"/>，当不存在时，返回 null</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetResponseDescription(this HttpContext httpContext, [NotNullWhen(true)] out ResponseDescription? description)
        {
            if (httpContext.Items.TryGetValue(typeof(ResponseDescription), out var storedObject)
                && storedObject is ResponseDescription storedDescription)
            {
                description = storedDescription;
                return true;
            }
            description = null;
            return false;
        }

        #endregion Public 方法
    }
}