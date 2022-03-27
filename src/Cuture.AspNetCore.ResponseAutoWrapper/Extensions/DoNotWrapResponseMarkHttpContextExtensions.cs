using System.Runtime.CompilerServices;

using Cuture.AspNetCore.ResponseAutoWrapper;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// 标记不包装响应的HttpContext拓展
/// </summary>
public static class DoNotWrapResponseMarkHttpContextExtensions
{
    //TODO add test for this

    #region Public 方法

    /// <summary>
    /// 标记不包装当前上下文的响应
    /// </summary>
    /// <param name="httpContext"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DoNotWrapResponse(this HttpContext httpContext)
    {
        if (!httpContext.Items.ContainsKey(Constants.HttpContextDoNotWrapResponseMarkKey))
        {
            httpContext.Items.Add(Constants.HttpContextDoNotWrapResponseMarkKey, string.Empty);
        }
    }

    /// <summary>
    /// 检查当前上下文是否标记了不包装响应
    /// </summary>
    /// <param name="httpContext"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSetDoNotWrapResponse(this HttpContext httpContext) => httpContext.Items.ContainsKey(Constants.HttpContextDoNotWrapResponseMarkKey);

    #endregion Public 方法
}