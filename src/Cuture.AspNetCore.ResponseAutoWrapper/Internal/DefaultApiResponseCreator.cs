using System;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal
{
    /// <summary>
    /// 默认的针对响应类型为 <see cref="ApiResponse{TData}"/> 的<inheritdoc cref="IResponseCreator"/>
    /// </summary>
    internal class DefaultApiResponseCreator : IResponseCreator<ApiResponse<object>>
    {
        #region Public 方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ApiResponse<object> Create(int code, object? data, string? message = null)
            => new() { Code = code, Data = data, Message = message };

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ApiResponse<object> Create(int code, Exception exception, string? message = null)
            => new() { Code = code, Message = message };

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ApiResponse<object> Create(int code, string? message = null)
            => new() { Code = code, Message = message };

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object CreateObject(int code, string? message = null, Exception? exception = null)
            => exception is null ? Create(code, message: message) : Create(code, exception: exception, message: message);

        #endregion Public 方法
    }
}