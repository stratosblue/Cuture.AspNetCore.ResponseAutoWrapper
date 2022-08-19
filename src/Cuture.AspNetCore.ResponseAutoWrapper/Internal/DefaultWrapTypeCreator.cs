using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal;

internal class DefaultWrapTypeCreator<TCode, TMessage> : IWrapTypeCreator<TCode, TMessage>
{
    #region Private 字段

    /// <summary>
    /// 创建包装类型的委托
    /// </summary>
    private readonly Func<Type, Type> _makeWrapTypeDelegate;

    /// <summary>
    /// 响应数据的泛型参数位置索引
    /// </summary>
    private readonly int _responseDataGenericTypeIndex = -1;

    /// <summary>
    /// 响应类型的泛型参数列表
    /// </summary>
    private readonly Type[] _responseTypeGenericArguments = Array.Empty<Type>();

    /// <summary>
    /// 类型是否应该包装检查委托
    /// </summary>
    private readonly Func<Type, bool> _shouldWrapCheckDelegate;

    /// <summary>
    /// 类型的包装策略缓存
    /// </summary>
    private readonly ConcurrentDictionary<IntPtr, bool> _typeWrapPolicyCache = new();

    #endregion Private 字段

    #region Public 属性

    /// <inheritdoc/>
    public Type? ResponseGenericType { get; }

    /// <inheritdoc/>
    public Type ResponseType { get; }

    #endregion Public 属性

    #region Public 构造函数

    public DefaultWrapTypeCreator(Type responseType, Type? responseGenericType)
    {
        ResponseType = responseType ?? throw new ArgumentNullException(nameof(responseType));
        ResponseGenericType = responseGenericType ??= (responseType.IsGenericType ? responseType.GetGenericTypeDefinition() : null);

        if (responseGenericType is null)
        {
            _shouldWrapCheckDelegate = ShouldWrapCheckWithOutGenericType;
            _makeWrapTypeDelegate = _ => ResponseType;
        }
        else
        {
            var genericTypeParameters = responseGenericType.GetTypeInfo().GenericTypeParameters.ToList();
            _responseDataGenericTypeIndex = genericTypeParameters.FindIndex(m => m.GetCustomAttribute<ResponseDataAttribute>() is not null);
            _responseTypeGenericArguments = responseType.GetGenericArguments();

            _shouldWrapCheckDelegate = ShouldWrapCheck;
            _makeWrapTypeDelegate = InternalMakeWrapType;
        }
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public Type MakeWrapType(Type type) => _makeWrapTypeDelegate(type);

    /// <inheritdoc/>
    public bool ShouldWrap(Type type) => _shouldWrapCheckDelegate(type);

    #endregion Public 方法

    #region Private 方法

    private Type InternalMakeWrapType(Type type)
    {
        var genericArguments = (Type[])_responseTypeGenericArguments.Clone();
        genericArguments[_responseDataGenericTypeIndex] = type;
        return ResponseGenericType!.MakeGenericType(genericArguments);
    }

    /// <summary>
    /// 检查类型是否需要包装（包含泛型检查）
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool ShouldWrapCheck(Type type)
    {
        if (_typeWrapPolicyCache.TryGetValue(type.TypeHandle.Value, out var shouldWrap))
        {
            return shouldWrap;
        }

        //返回值本身就是响应类型或其子类
        shouldWrap = !(type.IsAssignableTo(ResponseType)
                       || type.HasImplementedGenericSpecial(ResponseGenericType!, _responseTypeGenericArguments, _responseDataGenericTypeIndex));

        _typeWrapPolicyCache.TryAdd(type.TypeHandle.Value, shouldWrap);

        return shouldWrap;
    }

    /// <summary>
    /// 检查类型是否需要包装（不包含泛型检查）
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private bool ShouldWrapCheckWithOutGenericType(Type type)
    {
        //返回值本身就是响应类型或其子类
        return !type.IsAssignableTo(ResponseType);
    }

    #endregion Private 方法
}
