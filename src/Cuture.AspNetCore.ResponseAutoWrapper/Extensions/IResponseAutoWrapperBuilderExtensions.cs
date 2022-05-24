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
            _services = services ?? throw new ArgumentNullException(nameof(services));
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
        /// 添加一个 Wrapper
        /// </summary>
        /// <typeparam name="TWrapper"></typeparam>
        /// <param name="lifetime">注册DI容器的生命周期</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public WrapperBuilder<TResponse, TCode, TMessage> AddWrapper<TWrapper>(ServiceLifetime lifetime = ServiceLifetime.Singleton) where TWrapper : IWrapper<TResponse, TCode, TMessage>
        {
            var wrapperType = typeof(TWrapper);
            Type serviceType;
            if (wrapperType.IsAssignableTo(typeof(IActionResultWrapper<TResponse, TCode, TMessage>)))
            {
                serviceType = typeof(IActionResultWrapper<TResponse, TCode, TMessage>);
            }
            else if (wrapperType.IsAssignableTo(typeof(IExceptionWrapper<TResponse, TCode, TMessage>)))
            {
                serviceType = typeof(IExceptionWrapper<TResponse, TCode, TMessage>);
            }
            else if (wrapperType.IsAssignableTo(typeof(IInvalidModelStateWrapper<TResponse, TCode, TMessage>)))
            {
                serviceType = typeof(IInvalidModelStateWrapper<TResponse, TCode, TMessage>);
            }
            else if (wrapperType.IsAssignableTo(typeof(INotOKStatusCodeWrapper<TResponse, TCode, TMessage>)))
            {
                serviceType = typeof(INotOKStatusCodeWrapper<TResponse, TCode, TMessage>);
            }
            else
            {
                throw new ArgumentException($"{wrapperType} not implemented any available wrapper interface for response type {typeof(TResponse)}.");
            }

            _services.TryAddEnumerable(ServiceDescriptor.Describe(serviceType, wrapperType, lifetime));

            return this;
        }

        /// <summary>
        /// 尝试将派生自 <see cref="AbstractResponseWrapper{TResponse, TCode, TMessage}"/> 的类添加为多个 Wrapper
        /// </summary>
        /// <typeparam name="TWrapper"></typeparam>
        /// <returns></returns>
        public WrapperBuilder<TResponse, TCode, TMessage> AddWrappers<TWrapper>() where TWrapper : AbstractResponseWrapper<TResponse, TCode, TMessage>
        {
            _services.TryAddWrapper<TWrapper, TResponse, TCode, TMessage>();
            return this;
        }

        #endregion Public 方法
    }

    #endregion Public 类
}
