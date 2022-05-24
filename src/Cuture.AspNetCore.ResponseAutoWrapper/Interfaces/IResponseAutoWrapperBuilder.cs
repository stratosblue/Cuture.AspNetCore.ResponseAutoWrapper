using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// ResponseAutoWrapper构建器
/// </summary>
public interface IResponseAutoWrapperBuilder<TResponse, TCode, TMessage>
    where TResponse : class
{
    #region Public 属性

    /// <summary>
    /// <see cref="IServiceCollection"/>
    /// </summary>
    IServiceCollection Services { get; }

    #endregion Public 属性
}
