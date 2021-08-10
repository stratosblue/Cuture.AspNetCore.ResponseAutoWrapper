#if NET5_0_OR_GREATER
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authorization
{
    /// <summary>
    /// AutoWrapper AuthorizationMiddlewareResultHandler<para/>
    /// 将认证、授权失败的行为修改为设置状态码，并中断后续操作
    /// </summary>
    internal class AutoWrapperAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
#region Public 方法

        /// <inheritdoc/>
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            else if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            await next(context);
        }

#endregion Public 方法
    }
}
#endif