using System;

using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// AutoWrapper的 ApplicationBuilder 拓展
    /// </summary>
    public static class ApplicationBuilderResponseAutoWrapperExtensions
    {
        #region Public 方法

        /// <summary>
        /// 使用AutoWrapper中间件<para/>
        /// 包装请求中的异常，并有条件的包装响应状态码非 <see cref="StatusCodes.Status200OK"/> 的请求
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options">中间件配置</param>
        /// <returns></returns>
        public static IApplicationBuilder UseResponseAutoWrapper(this IApplicationBuilder app, ResponseAutoWrapMiddlewareOptions? options = null)
        {
            return app.UseMiddleware<ResponseAutoWrapMiddleware>(app.ApplicationServices, options ?? new ResponseAutoWrapMiddlewareOptions());
        }

        /// <summary>
        /// 使用AutoWrapper中间件<para/>
        /// 包装请求中的异常，并有条件的包装响应状态码非 <see cref="StatusCodes.Status200OK"/> 的请求
        /// </summary>
        /// <param name="app"></param>
        /// <param name="optionsSetupAction">中间件配置设置委托</param>
        /// <returns></returns>
        public static IApplicationBuilder UseResponseAutoWrapper(this IApplicationBuilder app, Action<ResponseAutoWrapMiddlewareOptions>? optionsSetupAction)
        {
            var options = new ResponseAutoWrapMiddlewareOptions();
            optionsSetupAction?.Invoke(options);

            return app.UseMiddleware<ResponseAutoWrapMiddleware>(app.ApplicationServices, options);
        }

        #endregion Public 方法
    }
}