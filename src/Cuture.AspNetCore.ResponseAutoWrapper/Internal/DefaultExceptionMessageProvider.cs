using System;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal;

/// <summary>
/// 默认的 <inheritdoc cref="IExceptionMessageProvider"/>
/// </summary>
internal class DefaultExceptionMessageProvider : IExceptionMessageProvider
{
    #region Public 方法

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ParseMessage(HttpContext httpContext, Exception exception)
    {
        return "Server Error";
    }

    #endregion Public 方法
}
