using System;

using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///
/// </summary>
public static class IResponseAutoWrapperBuilderExtensions
{
    #region Public 方法

    /// <summary>
    /// 配置使用的包装器
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="builder"></param>
    /// <param name="wrapperConfigureAction"></param>
    /// <returns></returns>
    public static IResponseAutoWrapperBuilder<TResponse, TCode, TMessage> ConfigureWrappers<TResponse, TCode, TMessage>(this IResponseAutoWrapperBuilder<TResponse, TCode, TMessage> builder,
                                                                                                                        Action<WrapperBuilder<TResponse, TCode, TMessage>> wrapperConfigureAction)
        where TResponse : class
    {
        wrapperConfigureAction(new WrapperBuilder<TResponse, TCode, TMessage>(builder.Services));
        return builder;
    }

    /// <summary>
    /// 添加默认的旧响应包装器（TCode 为 <see cref="int"/>，TMessage 为 <see cref="string"/> 的响应类型）
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    internal static IResponseAutoWrapperBuilder<GenericApiResponse<int, string, object>, int, string> AddDefaultLegacyWrappers(this IResponseAutoWrapperBuilder<GenericApiResponse<int, string, object>, int, string> builder)
    {
        new WrapperBuilder<GenericApiResponse<int, string, object>, int, string>(builder.Services).AddLegacyWrappers<DefaultResponseWrapper>();
        return builder;
    }

    #endregion Public 方法

    #region Public 类

    /// <summary>
    /// Wrapper构建器
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public sealed class WrapperBuilder<TResponse, TCode, TMessage>
        where TResponse : class
    {
        #region Private 字段

        private readonly IServiceCollection _services;

        #endregion Private 字段

        #region Public 构造函数

        internal WrapperBuilder(IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            _services = services;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <summary>
        /// 尝试将派生自 <see cref="LegacyCompatibleResponseWrapper{TResponseTMessage}"/> 的类添加为多个 Wrapper
        /// </summary>
        /// <returns></returns>
        public WrapperBuilder<TResponse, TCode, TMessage> AddLegacyWrappers<TLegacyCompatibleWrapper>()
            where TLegacyCompatibleWrapper : LegacyCompatibleResponseWrapper<TResponse>
        {
            _services.TryAddWrapper<TLegacyCompatibleWrapper, TResponse, int, string>();
            return this;
        }

        /// <summary>
        /// 检查 <typeparamref name="TWrapper"/> 实现的包装接口，将其添加为对应的包装器
        /// </summary>
        /// <typeparam name="TWrapper"></typeparam>
        /// <param name="lifetime">注册DI容器的生命周期</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public WrapperBuilder<TResponse, TCode, TMessage> AddWrapper<TWrapper>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TWrapper : IWrapper<TResponse, TCode, TMessage>
        {
            bool hasAdded = false;
            if (TryAddWrapper<TWrapper, IActionResultWrapper<TResponse, TCode, TMessage>>(lifetime))
            {
                hasAdded = true;
            }
            if (TryAddWrapper<TWrapper, IExceptionWrapper<TResponse, TCode, TMessage>>(lifetime))
            {
                hasAdded = true;
            }
            if (TryAddWrapper<TWrapper, IInvalidModelStateWrapper<TResponse, TCode, TMessage>>(lifetime))
            {
                hasAdded = true;
            }
            if (TryAddWrapper<TWrapper, INotOKStatusCodeWrapper<TResponse, TCode, TMessage>>(lifetime))
            {
                hasAdded = true;
            }
            if (!hasAdded)
            {
                throw new ArgumentException($"{typeof(TWrapper)} not implemented any available wrapper interface for response type {typeof(TResponse)}.");
            }
            return this;
        }

        /// <summary>
        /// 将 <typeparamref name="TWrapper"/> 添加为 <typeparamref name="TWrapperInterface"/> 包装器
        /// </summary>
        /// <typeparam name="TWrapper"></typeparam>
        /// <typeparam name="TWrapperInterface"></typeparam>
        /// <param name="lifetime">注册DI容器的生命周期</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public WrapperBuilder<TResponse, TCode, TMessage> AddWrapper<TWrapper, TWrapperInterface>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TWrapper : IWrapper<TResponse, TCode, TMessage>, TWrapperInterface
            where TWrapperInterface : IWrapper<TResponse, TCode, TMessage>
        {
            if (!TryAddWrapper<TWrapper, TWrapperInterface>(lifetime))
            {
                throw new ArgumentException($"{typeof(TWrapper)} not implemented wrapper interface {typeof(TWrapperInterface)} for response type {typeof(TResponse)}.");
            }

            return this;
        }

        /// <summary>
        /// 尝试将派生自 <see cref="AbstractResponseWrapper{TResponse, TCode, TMessage}"/> 的类添加为多个 Wrapper
        /// </summary>
        /// <typeparam name="TWrapper"></typeparam>
        /// <param name="lifetime">注册DI容器的生命周期</param>
        /// <returns></returns>
        public WrapperBuilder<TResponse, TCode, TMessage> AddWrappers<TWrapper>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TWrapper : AbstractResponseWrapper<TResponse, TCode, TMessage>
        {
            _services.TryAddWrapper<TWrapper, TResponse, TCode, TMessage>(lifetime);
            return this;
        }

        /// <summary>
        /// 尝试将 <typeparamref name="TWrapper"/> 添加为 <typeparamref name="TWrapperInterface"/> 包装器
        /// </summary>
        /// <typeparam name="TWrapper"></typeparam>
        /// <typeparam name="TWrapperInterface"></typeparam>
        /// <param name="lifetime">注册DI容器的生命周期</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool TryAddWrapper<TWrapper, TWrapperInterface>(ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TWrapper : IWrapper<TResponse, TCode, TMessage>
            where TWrapperInterface : IWrapper<TResponse, TCode, TMessage>
        {
            var wrapperType = typeof(TWrapper);
            if (wrapperType.IsAssignableTo(typeof(IActionResultWrapper<TResponse, TCode, TMessage>)))
            {
                _services.TryAddEnumerable(ServiceDescriptor.Describe(typeof(TWrapperInterface), wrapperType, lifetime));
                return true;
            }

            return false;
        }

        #endregion Public 方法
    }

    #endregion Public 类
}
