namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 包装器
/// </summary>
public interface IWrapper
{
}

/// <summary>
/// 包装器
/// </summary>
/// <typeparam name="TResponse">统一响应类型</typeparam>
public interface IWrapper<TResponse> : IWrapper
{
}
