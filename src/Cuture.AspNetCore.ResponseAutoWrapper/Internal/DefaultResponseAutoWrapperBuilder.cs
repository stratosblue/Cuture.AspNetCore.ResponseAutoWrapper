using System;

using Microsoft.Extensions.DependencyInjection;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

internal sealed class DefaultResponseAutoWrapperBuilder<TResponse, TCode, TMessage>
    : IResponseAutoWrapperBuilder<TResponse, TCode, TMessage>
    where TResponse : class
{
    #region Public 属性

    public IServiceCollection Services { get; }

    #endregion Public 属性

    #region Public 构造函数

    public DefaultResponseAutoWrapperBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    #endregion Public 构造函数
}