using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal
{
    /// <inheritdoc/>
    internal class DefaultResponseDirectWriter : IResponseDirectWriter
    {
        #region Public 方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task WriteAsync(HttpContext httpContext, object? value) => httpContext.Response.WriteAsJsonAsync(value, httpContext.RequestAborted);

        #endregion Public 方法
    }
}