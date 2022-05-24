﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 默认的响应包装器<para/>
/// TCode 为 <see cref="int"/><para/>
/// TMessage 为 <see cref="string"/>
/// </summary>
internal sealed class DefaultResponseWrapper : LegacyCompatibleResponseWrapper<GenericApiResponse<int, string, object>>
{
    #region Public 构造函数

    /// <inheritdoc cref="DefaultResponseWrapper"/>
    public DefaultResponseWrapper(IWrapTypeCreator<int, string> wrapTypeCreator,
                                  IOptions<ResponseAutoWrapperOptions> optionsAccessor,
                                  IOptions<LegacyCompatibleResponseWrapperOptions> wrapperOptionsAccessor)
        : base(wrapTypeCreator, optionsAccessor, wrapperOptionsAccessor)
    {
    }

    #endregion Public 构造函数

    #region CreateResponse

    /// <inheritdoc/>
    protected override GenericApiResponse<int, string, object>? CreateResponse(int code) => new(code);

    /// <inheritdoc/>
    protected override GenericApiResponse<int, string, object>? CreateResponse(int code, string? message) => new(code) { Message = message };

    /// <inheritdoc/>
    protected override GenericApiResponse<int, string, object>? CreateResponse(int code, string? message, object? data) => new(code) { Data = data, Message = message };

    #endregion CreateResponse
}
