using System;
using System.Linq;
using System.Reflection;

using Cuture.AspNetCore.ResponseAutoWrapper;
using Cuture.AspNetCore.ResponseAutoWrapper.Internal;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Authorization;
#endif

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// AutoWrapper的 ServiceCollection 拓展
/// </summary>
public static class ServiceCollectionResponseAutoWrapperExtensions
{
    #region Public 方法

    /// <inheritdoc cref="AddResponseAutoWrapper(IServiceCollection, Action{ResponseAutoWrapperOptions})"/>
    public static IResponseAutoWrapperBuilder<GenericApiResponse<int, string, object>, int, string> AddResponseAutoWrapper(this IServiceCollection services)
    {
        return services.AddResponseAutoWrapper<int, string>(null).AddDefaultLegacyWrappers();
    }

    /// <summary>
    /// 添加响应自动包装器<para/>
    /// TCode 为 <see cref="int"/><para/>
    /// TMessage 为 <see cref="string"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="optionsSetupAction"></param>
    /// <returns></returns>
    public static IResponseAutoWrapperBuilder<GenericApiResponse<int, string, object>, int, string> AddResponseAutoWrapper(this IServiceCollection services, Action<ResponseAutoWrapperOptions>? optionsSetupAction)
    {
        return services.AddResponseAutoWrapper<int, string>(optionsSetupAction).AddDefaultLegacyWrappers();
    }

    /// <inheritdoc cref="AddResponseAutoWrapper{TCode, TMessage}(IServiceCollection, Action{ResponseAutoWrapperOptions}?)"/>
    public static IResponseAutoWrapperBuilder<GenericApiResponse<TCode, TMessage, object>, TCode, TMessage> AddResponseAutoWrapper<TCode, TMessage>(this IServiceCollection services) => services.AddResponseAutoWrapper<TCode, TMessage>(null);

    /// <summary>
    /// 添加响应自动包装器<para/>
    /// 使用默认的响应类型<see cref="GenericApiResponse{TCode, TMessage, TData}"/>
    /// </summary>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="services"></param>
    /// <param name="optionsSetupAction"></param>
    /// <returns></returns>
    public static IResponseAutoWrapperBuilder<GenericApiResponse<TCode, TMessage, object>, TCode, TMessage> AddResponseAutoWrapper<TCode, TMessage>(this IServiceCollection services, Action<ResponseAutoWrapperOptions>? optionsSetupAction)
    {
        var options = new ResponseAutoWrapperOptions();

        optionsSetupAction?.Invoke(options);

        return services.AddResponseAutoWrapper<GenericApiResponse<TCode, TMessage, object>, TCode, TMessage>(options);
    }

    /// <inheritdoc cref="AddResponseAutoWrapper{TResponse,TCode, TMessage}(IServiceCollection, ResponseAutoWrapperOptions)"/>
    public static IResponseAutoWrapperBuilder<TResponse, TCode, TMessage> AddResponseAutoWrapper<TResponse, TCode, TMessage>(this IServiceCollection services)
        where TResponse : class
    {
        return services.AddResponseAutoWrapper<TResponse, TCode, TMessage>(new ResponseAutoWrapperOptions());
    }

    /// <inheritdoc cref="AddResponseAutoWrapper{TResponse, TCode, TMessage}(IServiceCollection, ResponseAutoWrapperOptions)"/>
    public static IResponseAutoWrapperBuilder<TResponse, TCode, TMessage> AddResponseAutoWrapper<TResponse, TCode, TMessage>(this IServiceCollection services, Action<ResponseAutoWrapperOptions>? optionsSetupAction)
        where TResponse : class
    {
        var options = new ResponseAutoWrapperOptions();

        optionsSetupAction?.Invoke(options);

        return services.AddResponseAutoWrapper<TResponse, TCode, TMessage>(options);
    }

    /// <summary>
    /// 添加响应自动包装器<para/>
    /// 使用指定的响应类型 <typeparamref name="TResponse"/> 和 <typeparamref name="TCode"/>、<typeparamref name="TMessage"/>
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IResponseAutoWrapperBuilder<TResponse, TCode, TMessage> AddResponseAutoWrapper<TResponse, TCode, TMessage>(this IServiceCollection services, ResponseAutoWrapperOptions options)
        where TResponse : class
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
        services.SetupActionResultPolicyTagAppModelConvention<TResponse, TCode, TMessage>(options.ActionNoWrapPredicate, options.DisableOpenAPISupport);

        //Add action result filter
        services.TryAddSingleton<ResponseAutoWrapResultFilter<TResponse, TCode, TMessage>>();

        //setup action result filter
        services.PostConfigure<MvcOptions>(options =>
        {
            options.Filters.AddService<ResponseAutoWrapResultFilter<TResponse, TCode, TMessage>>(Constants.DefaultFilterOrder);
        });

#if NET5_0_OR_GREATER
        if (options.HandleAuthorizationResult)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IAuthorizationMiddlewareResultHandler, AutoWrapperAuthorizationMiddlewareResultHandler>());
        }
#endif

        if (options.HandleInvalidModelState)
        {
            services.SetupInvalidModelStateWrapper<TResponse, TCode, TMessage>();
        }

        services.TryAddSingleton<ResponseAutoWrapperWorkDelegateCollection>(serviceProvider =>
        {
            var exceptionWrapper = serviceProvider.GetRequiredService<IExceptionWrapper<TResponse, TCode, TMessage>>();
            var notOKStatusCodeWrapper = serviceProvider.GetRequiredService<INotOKStatusCodeWrapper<TResponse, TCode, TMessage>>();

            return new ResponseAutoWrapperWorkDelegateCollection(exceptionWrapper.Wrap,
                                                                 notOKStatusCodeWrapper.Wrap);
        });

        return new DefaultResponseAutoWrapperBuilder<TResponse, TCode, TMessage>(services);
    }

    #endregion Public 方法

    #region Internal 方法

    internal static void TryAddWrapper<TResponseWrapper, TResponse, TCode, TMessage>(this IServiceCollection services)
        where TResponseWrapper : AbstractResponseWrapper<TResponse, TCode, TMessage>
        where TResponse : class
    {
        static TResponseWrapper GetDefaultResponseWrapper(IServiceProvider serviceProvider) => serviceProvider.GetRequiredService<TResponseWrapper>();

        services.TryAddSingleton<TResponseWrapper>();
        services.TryAddSingleton<IActionResultWrapper<TResponse, TCode, TMessage>>(GetDefaultResponseWrapper);
        services.TryAddSingleton<IExceptionWrapper<TResponse, TCode, TMessage>>(GetDefaultResponseWrapper);
        services.TryAddSingleton<INotOKStatusCodeWrapper<TResponse, TCode, TMessage>>(GetDefaultResponseWrapper);
        services.TryAddSingleton<IInvalidModelStateWrapper<TResponse, TCode, TMessage>>(GetDefaultResponseWrapper);
    }

    #endregion Internal 方法

    #region Private 方法

    /// <summary>
    /// 设置ActionResult处理策略标记AppModelConvention
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="services"></param>
    /// <param name="actionNoWrapPredicate"></param>
    /// <param name="disableOpenAPISupport"></param>
    /// <returns></returns>
    private static void SetupActionResultPolicyTagAppModelConvention<TResponse, TCode, TMessage>(this IServiceCollection services,
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

        //启用OpenAPI支持时，TResponse必须是一个包含object泛型参数的泛型实现类，且object对应的泛型参数需要标记 ResponseDataAttribute
        if (!disableOpenAPISupport
            && !CheckResponseTypeGenericParameter(responseType, out responseGenericType))
        {
            throw new ArgumentException($"When enabled OpenAPI Support. Response type must be a generic type and has a generic type argument \"{typeof(object)}\" which has attribute \"{typeof(ResponseDataAttribute)}\".");
        }

        services.TryAddSingleton<IWrapTypeCreator<TCode, TMessage>>(new DefaultWrapTypeCreator<TCode, TMessage>(responseType, responseGenericType));

        services.TryAddEnumerable(ServiceDescriptor.Transient<IPostConfigureOptions<MvcOptions>, MvcOptionsPostConfigureOptions>(CreateMvcOptionsPostConfigureOptions));

        MvcOptionsPostConfigureOptions CreateMvcOptionsPostConfigureOptions(IServiceProvider serviceProvider)
        {
            return new MvcOptionsPostConfigureOptions(Options.Options.DefaultName, options =>
            {
                var wrapTypeCreator = serviceProvider.GetRequiredService<IWrapTypeCreator<TCode, TMessage>>();

                IApplicationModelConvention convention = disableOpenAPISupport
                                                            ? new ActionResultPolicyTagAppModelConvention<TCode, TMessage>(wrapTypeCreator, actionNoWrapPredicate)
                                                            : new OpenAPISupportAppModelConvention<TCode, TMessage>(wrapTypeCreator, actionNoWrapPredicate);

                options.Conventions.Add(convention);
            });
        }

        static bool CheckResponseTypeGenericParameter(Type responseType, out Type? responseGenericType)
        {
            if (!responseType.IsGenericType)
            {
                responseGenericType = null;
                return false;
            }
            responseGenericType = responseType.GetGenericTypeDefinition();
            var genericTypeParameters = responseGenericType.GetTypeInfo().GenericTypeParameters.ToList();
            var responseDataTypeIndex = genericTypeParameters.FindIndex(m => m.GetCustomAttribute<ResponseDataAttribute>() is not null);

            if (responseDataTypeIndex == -1)
            {
                return false;
            }

            var genericParameterTypes = responseType.GetGenericArguments();

            return genericParameterTypes[responseDataTypeIndex] == typeof(object);
        }
    }

    /// <summary>
    /// 设置无效模型状态包装器
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TCode"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    /// <param name="services"></param>
    private static void SetupInvalidModelStateWrapper<TResponse, TCode, TMessage>(this IServiceCollection services) where TResponse : class
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient<IPostConfigureOptions<ApiBehaviorOptions>, ApiBehaviorOptionsPostConfigureOptions>(CreateApiBehaviorOptionsPostConfigureOptions));

        static ApiBehaviorOptionsPostConfigureOptions CreateApiBehaviorOptionsPostConfigureOptions(IServiceProvider serviceProvider)
        {
            return new ApiBehaviorOptionsPostConfigureOptions(Options.Options.DefaultName, options =>
            {
                var wrapper = serviceProvider.GetRequiredService<IInvalidModelStateWrapper<TResponse, TCode, TMessage>>();

                var originInvalidModelStateResponseFactory = options.InvalidModelStateResponseFactory;

                options.InvalidModelStateResponseFactory = context => wrapper.Wrap(context) is TResponse response
                                                                                    ? new OkObjectResult(response)
                                                                                    : originInvalidModelStateResponseFactory(context);
            });
        }
    }

    #endregion Private 方法
}
