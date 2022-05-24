using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Http;

/// <summary>
/// 响应描述的HttpContext拓展
/// </summary>
public static class ResponseDescriptionHttpContextExtensions
{
    #region Public 方法

    /// <inheritdoc cref="DescribeResponse{TCode, TMessage}(HttpContext, TCode, TMessage)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DescribeResponse<TCode, TMessage>(this HttpContext httpContext, TCode code) => httpContext.Items[typeof(ResponseDescription<,>)] = new ResponseDescription<TCode, TMessage>(code);

    /// <summary>
    /// 描述响应<para/>
    /// 通过向 <see cref="HttpContext.Items"/> 添加 <see cref="ResponseDescription{TCode, TMessage}"/> 以描述响应
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="code">响应码</param>
    /// <param name="message">响应消息</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DescribeResponse<TCode, TMessage>(this HttpContext httpContext, TCode code, TMessage message) => httpContext.Items[typeof(ResponseDescription<,>)] = new ResponseDescription<TCode, TMessage>(code, message);

    /// <summary>
    /// 尝试获取描述响应<para/>
    /// 从 <see cref="HttpContext.Items"/> 中读取 <see cref="ResponseDescription{TCode, TMessage}"/>
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns>返回 <see cref="HttpContext.Items"/> 中的 <see cref="ResponseDescription{TCode, TMessage}"/>，当不存在时，返回 null</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ResponseDescription<TCode, TMessage>? GetResponseDescription<TCode, TMessage>(this HttpContext httpContext)
    {
        if (httpContext.Items.TryGetValue(typeof(ResponseDescription<,>), out var storedObject))
        {
            if (storedObject is ResponseDescription<TCode, TMessage> storedDescription)
            {
                return storedDescription;
            }
            throw DescriptionTypeNotMatchException<TCode, TMessage>();
        }

        return null;
    }

    /// <summary>
    /// <inheritdoc cref="GetResponseDescription{TCode, TMessage}(HttpContext)"/>
    /// </summary>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="executingContext"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ResponseDescription<TCode, TMessage>? GetResponseDescription<TCode, TMessage>(this ResultExecutingContext executingContext)
    {
        return executingContext.HttpContext.GetResponseDescription<TCode, TMessage>();
    }

    /// <summary>
    /// 尝试获取描述响应<para/>
    /// 从 <see cref="HttpContext.Items"/> 中读取 <see cref="ResponseDescription{TCode, TMessage}"/>
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="description"></param>
    /// <returns>返回 <see cref="HttpContext.Items"/> 中的 <see cref="ResponseDescription{TCode, TMessage}"/>，当不存在时，返回 null</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetResponseDescription<TCode, TMessage>(this HttpContext httpContext, [NotNullWhen(true)] out ResponseDescription<TCode, TMessage>? description)
    {
        if (httpContext.Items.TryGetValue(typeof(ResponseDescription<,>), out var storedObject))
        {
            if (storedObject is ResponseDescription<TCode, TMessage> storedDescription)
            {
                description = storedDescription;
                return true;
            }
            throw DescriptionTypeNotMatchException<TCode, TMessage>();
        }

        description = null;
        return false;
    }

    /// <summary>
    /// <inheritdoc cref="TryGetResponseDescription{TCode, TMessage}(HttpContext, out ResponseDescription{TCode, TMessage}?)"/>
    /// </summary>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="executingContext"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetResponseDescription<TCode, TMessage>(this ResultExecutingContext executingContext, [NotNullWhen(true)] out ResponseDescription<TCode, TMessage>? description)
    {
        return executingContext.HttpContext.TryGetResponseDescription(out description);
    }

    #endregion Public 方法

    #region Private 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Exception DescriptionTypeNotMatchException<TCode, TMessage>()
    {
        return new InvalidOperationException($"The http context has description object. But it's not the instance of {typeof(ResponseDescription<TCode, TMessage>)}. This situation is likely to be the description type use error. Please check it.");
    }

    #endregion Private 方法
}
