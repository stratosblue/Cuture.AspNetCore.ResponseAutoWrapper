using System.Runtime.CompilerServices;

using Cuture.AspNetCore.ResponseAutoWrapper;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// 标记不包装响应的HttpContext拓展
/// </summary>
public static class DoNotWrapMarkHttpContextExtensions
{
    //TODO add test for this

    #region Public 方法

    /// <summary>
    /// 标记不包装当前上下文的响应
    /// </summary>
    /// <param name="httpContext"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DoNotWrap(this HttpContext httpContext)
    {
        if (!httpContext.Items.ContainsKey(Constants.HttpContextDoNotWrapMarkKey))
        {
            httpContext.Items.Add(Constants.HttpContextDoNotWrapMarkKey, string.Empty);
        }
    }

    /// <summary>
    /// 检查当前上下文是否标记了不包装响应
    /// </summary>
    /// <param name="httpContext"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSetDoNotWrap(this HttpContext httpContext) => httpContext.Items.ContainsKey(Constants.HttpContextDoNotWrapMarkKey);

    #endregion Public 方法
}