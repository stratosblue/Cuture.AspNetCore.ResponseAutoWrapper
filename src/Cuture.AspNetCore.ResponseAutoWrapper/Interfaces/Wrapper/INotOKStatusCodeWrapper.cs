using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// <see cref="HttpResponse.StatusCode"/> 非 <see cref="StatusCodes.Status200OK"/> 时的响应包装器
/// </summary>
/// <typeparam name="TResponse">统一响应类型</typeparam>
public interface INotOKStatusCodeWrapper<TResponse> : IWrapper<TResponse>
{
    /// <summary>
    /// 通过 <see cref="HttpContext"/> 返回包装后的统一响应
    /// </summary>
    /// <param name="context"></param>
    /// <returns>包装后的统一响应类型对象，返回 null 时，不做处理</returns>
    TResponse? Wrap(HttpContext context);
}
