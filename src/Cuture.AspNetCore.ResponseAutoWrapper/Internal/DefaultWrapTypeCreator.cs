using System;
using System.Collections.Concurrent;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal
{
    internal class DefaultWrapTypeCreator : IWrapTypeCreator
    {
        #region Private 字段

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
            ResponseGenericType = responseGenericType;

            _shouldWrapCheckDelegate = responseGenericType is null
                                            ? ShouldWrapCheckWithOutGenericType
                                            : ShouldWrapCheck;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public Type MakeWrapType(Type type)
            => ResponseGenericType is null ? ResponseType : ResponseGenericType.MakeGenericType(type);

        /// <inheritdoc/>
        public bool ShouldWrap(Type type) => _shouldWrapCheckDelegate(type);

        #endregion Public 方法

        #region Private 方法

        private bool ShouldWrapCheck(Type type)
        {
            if (_typeWrapPolicyCache.TryGetValue(type.TypeHandle.Value, out var shouldWrap))
            {
                return shouldWrap;
            }

            //返回值本身就是响应类型或其子类
            shouldWrap = !(type.IsAssignableTo(ResponseType)
                           || type.HasImplementedGeneric(ResponseGenericType!));

            _typeWrapPolicyCache.TryAdd(type.TypeHandle.Value, shouldWrap);

            return shouldWrap;
        }

        private bool ShouldWrapCheckWithOutGenericType(Type type)
        {
            //返回值本身就是响应类型或其子类
            if (type.IsAssignableTo(ResponseType))
            {
                return false;
            }
            return true;
        }

        #endregion Private 方法
    }
}