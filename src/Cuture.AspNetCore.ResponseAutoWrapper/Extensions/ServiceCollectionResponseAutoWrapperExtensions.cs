using System;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Cuture.AspNetCore.ResponseAutoWrapper;
using Cuture.AspNetCore.ResponseAutoWrapper.Internal;

#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Authorization;
#endif

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// AutoWrapper的 ServiceCollection 拓展
    /// </summary>
    public static class ServiceCollectionResponseAutoWrapperExtensions
    {
        #region Public 方法

        /// <inheritdoc cref="AddResponseAutoWrapper(IServiceCollection, Action{ResponseAutoWrapperOptions}?)"/>
        public static IServiceCollection AddResponseAutoWrapper(this IServiceCollection services) => services.AddResponseAutoWrapper(null);

        /// <summary>
        /// 添加响应自动包装器<para/>
        /// 使用默认的响应类型<see cref="ApiResponse"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsSetupAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddResponseAutoWrapper(this IServiceCollection services, Action<ResponseAutoWrapperOptions>? optionsSetupAction)
        {
            var options = new ResponseAutoWrapperOptions();

            optionsSetupAction?.Invoke(options);

            return services.AddResponseAutoWrapper<ApiResponse<object>, DefaultApiResponseCreator>(options);
        }

        /// <inheritdoc cref="AddResponseAutoWrapper{TResponse}(IServiceCollection, ResponseAutoWrapperOptions)"/>
        public static IServiceCollection AddResponseAutoWrapper<TResponse>(this IServiceCollection services)
            where TResponse : notnull, new()
        {
            return services.AddResponseAutoWrapper<TResponse>(new ResponseAutoWrapperOptions());
        }

        /// <inheritdoc cref="AddResponseAutoWrapper{TResponse}(IServiceCollection, ResponseAutoWrapperOptions)"/>
        public static IServiceCollection AddResponseAutoWrapper<TResponse>(this IServiceCollection services, Action<ResponseAutoWrapperOptions>? optionsSetupAction)
            where TResponse : notnull, new()
        {
            var options = new ResponseAutoWrapperOptions();

            optionsSetupAction?.Invoke(options);

            return services.AddResponseAutoWrapper<TResponse>(options);
        }

        /// <summary>
        /// 添加响应自动包装器<para/>
        /// 使用指定的响应类型 <typeparamref name="TResponse"/> ，并使用默认的 <see cref="IResponseCreator{TResponse}"/><para/>
        /// <typeparamref name="TResponse"/> 需要按需实现<see cref="ISetResponseCode"/>、<see cref="ISetResponseMessage"/>、<see cref="ISetResponseData"/>、<see cref="ISetResponseException"/>、
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddResponseAutoWrapper<TResponse>(this IServiceCollection services, ResponseAutoWrapperOptions options)
            where TResponse : notnull, new()
        {
            return services.AddResponseAutoWrapper<TResponse, DefaultResponseCreator<TResponse>>(options);
        }

        /// <inheritdoc cref="AddResponseAutoWrapper{TResponse, TResponseCreator}(IServiceCollection, ResponseAutoWrapperOptions)"/>
        public static IServiceCollection AddResponseAutoWrapper<TResponse, TResponseCreator>(this IServiceCollection services)
            where TResponse : notnull
            where TResponseCreator : IResponseCreator<TResponse>
        {
            return services.AddResponseAutoWrapper<TResponse, TResponseCreator>(new ResponseAutoWrapperOptions());
        }

        /// <inheritdoc cref="AddResponseAutoWrapper{TResponse, TResponseCreator}(IServiceCollection, ResponseAutoWrapperOptions)"/>
        public static IServiceCollection AddResponseAutoWrapper<TResponse, TResponseCreator>(this IServiceCollection services, Action<ResponseAutoWrapperOptions>? optionsSetupAction)
            where TResponse : notnull
            where TResponseCreator : IResponseCreator<TResponse>
        {
            var options = new ResponseAutoWrapperOptions();

            optionsSetupAction?.Invoke(options);

            return services.AddResponseAutoWrapper<TResponse, TResponseCreator>(options);
        }

        /// <summary>
        /// 添加响应自动包装器<para/>
        /// 使用指定的响应类型 <typeparamref name="TResponse"/> 和响应创建器 <typeparamref name="TResponseCreator"/>
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <typeparam name="TResponseCreator"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddResponseAutoWrapper<TResponse, TResponseCreator>(this IServiceCollection services, ResponseAutoWrapperOptions options)
        where TResponse : notnull
        where TResponseCreator : IResponseCreator<TResponse>
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            services.AddControllers();

            //Setup how to handle actions
            services.SetupActionResultPolicyTagAppModelConvention<TResponse>(options.ActionNoWrapPredicate, options.DisableOpenAPISupport);

            //Add IResponseCreator
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IResponseCreator<TResponse>), typeof(TResponseCreator)));
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IResponseCreator), typeof(TResponseCreator)));

            services.TryAddSingleton<IMessageProvider, DefaultMessageProvider>();
            services.TryAddSingleton<IResponseDirectWriter, DefaultResponseDirectWriter>();

            //Add action result filter
            services.TryAddSingleton<ResponseAutoWrapResultFilter<TResponse>>();

            //setup action result filter
            services.PostConfigure<MvcOptions>(options =>
            {
                options.Filters.AddService<ResponseAutoWrapResultFilter<TResponse>>(Constants.DefaultFilterOrder);
            });

#if NET5_0_OR_GREATER
            if (options.HandleAuthorizationResult)
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton<IAuthorizationMiddlewareResultHandler, AutoWrapperAuthorizationMiddlewareResultHandler>());
            }
#endif

            if (options.HandleInvalidModelState)
            {
                services.TryAddSingleton<IInvalidModelStateResponseFormatter<TResponse>, DefaultInvalidModelStateResponseFormatter<TResponse>>();

                services.TryAddEnumerable(ServiceDescriptor.Transient<IPostConfigureOptions<ApiBehaviorOptions>, ApiBehaviorOptionsPostConfigureOptions>(CreateApiBehaviorOptionsPostConfigureOptions));
            }

            return services;

            static ApiBehaviorOptionsPostConfigureOptions CreateApiBehaviorOptionsPostConfigureOptions(IServiceProvider serviceProvider)
            {
                return new ApiBehaviorOptionsPostConfigureOptions(Options.Options.DefaultName, options =>
                {
                    var formatter = serviceProvider.GetRequiredService<IInvalidModelStateResponseFormatter<TResponse>>();
                    options.InvalidModelStateResponseFactory = context => new OkObjectResult(formatter.Handle(context));
                });
            }
        }

        #endregion Public 方法

        #region Private 方法

        private static IServiceCollection SetupActionResultPolicyTagAppModelConvention<TResponse>(this IServiceCollection services,
                                                                                                  Func<MemberInfo, bool> actionNoWrapPredicate,
                                                                                                  bool disableOpenAPISupport)
        {
            var responseType = typeof(TResponse);

            if (responseType.IsAbstract
                || responseType.IsInterface)
            {
                throw new ArgumentException("Response type must not be abstract or interface.");
            }

            Type? responseGenericType = null;

            if (responseType.IsGenericType
                && responseType.GenericTypeArguments.Length == 1
                && responseType.GenericTypeArguments[0] == typeof(object))
            {
                responseGenericType = responseType.GetGenericTypeDefinition();
            }

            //启用OpenAPI支持时，TResponse必须是一个泛型参数为object的泛型实现类
            if (!disableOpenAPISupport
                && responseGenericType is null)
            {
                throw new ArgumentException("When enabled OpenAPI Support. Response type must be a generic type and only one generic type argument - object.");
            }

            services.TryAddSingleton<IWrapTypeCreator>(new DefaultWrapTypeCreator(responseType, responseGenericType));

            services.TryAddEnumerable(ServiceDescriptor.Transient<IPostConfigureOptions<MvcOptions>, MvcOptionsPostConfigureOptions>(CreateMvcOptionsPostConfigureOptions));

            return services;

            MvcOptionsPostConfigureOptions CreateMvcOptionsPostConfigureOptions(IServiceProvider serviceProvider)
            {
                return new MvcOptionsPostConfigureOptions(Options.Options.DefaultName, options =>
                {
                    var wrapTypeCreator = serviceProvider.GetRequiredService<IWrapTypeCreator>();

                    IApplicationModelConvention convention = disableOpenAPISupport
                                                                ? new ActionResultPolicyTagAppModelConvention(wrapTypeCreator, actionNoWrapPredicate)
                                                                : new OpenAPISupportAppModelConvention(wrapTypeCreator, actionNoWrapPredicate);

                    options.Conventions.Add(convention);
                });
            }
        }

        #endregion Private 方法
    }
}