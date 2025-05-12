﻿using System.Linq;
using System.Threading.Tasks;

namespace System;

internal static class TypeExtensions
{
    #region Public 方法

    /// <summary>
    /// 查找 <paramref name="type"/> 所实现的泛型类定义 <paramref name="genericClassType"/> 的泛型参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericClassType">typeof(GenericClassName&lt;&gt;)</param>
    /// <returns></returns>
    public static Type[] FindImplementedGenericClassArguments(this Type type, Type genericClassType)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(genericClassType);

        if (!genericClassType.IsGenericTypeDefinition
            || genericClassType.IsInterface)
        {
            throw new ArgumentException("must be a generic type definition class.", nameof(genericClassType));
        }

        while (type != null && type != typeof(object))
        {
            if (type.IsTheRawGenericType(genericClassType))
            {
                return type.GetGenericArguments();
            }
            type = type.BaseType!;
        }

        return [];
    }

    /// <summary>
    /// 查找所有 <paramref name="type"/> 所实现的泛型接口定义 <paramref name="genericInterfaceType"/> 的泛型参数
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericInterfaceType">typeof(GenericInterfaceName&lt;&gt;)</param>
    /// <returns></returns>
    public static Type[][] FindImplementedGenericInterfaceArguments(this Type type, Type genericInterfaceType)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(genericInterfaceType);

        if (!genericInterfaceType.IsGenericTypeDefinition
            || !genericInterfaceType.IsInterface)
        {
            throw new ArgumentException("must be a generic type definition interface.", nameof(genericInterfaceType));
        }

        return type.GetInterfaces()
                   .Where(m => m.IsTheRawGenericType(genericInterfaceType))
                   .Select(m => m.GetGenericArguments())
                   .ToArray();
    }

    /// <summary>
    /// 判断类型 <paramref name="type"/> 是否是指定泛型类型 <paramref name="generic"/> 的子类型
    /// </summary>
    /// <param name="type"></param>
    /// <param name="generic">typeof(GenericTypeName&lt;&gt;)</param>
    /// <returns>是否是泛型接口的子类型</returns>
    public static bool HasImplementedGeneric(this Type type, Type generic)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(generic);

        if (!generic.IsGenericTypeDefinition)
        {
            throw new ArgumentException("must be a generic type definition.", nameof(generic));
        }

        if (generic.IsInterface)
        {
            return type.GetInterfaces().Any(m => m.IsTheRawGenericType(generic));
        }
        else
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsTheRawGenericType(generic))
                {
                    return true;
                }
                type = type.BaseType!;
            }
        }

        return false;
    }

    /// <summary>
    /// 判断类型 <paramref name="originType"/> 是否是指定泛型类型 <paramref name="genericType"/> 的子类型
    /// </summary>
    /// <param name="originType"></param>
    /// <param name="genericType">typeof(GenericTypeName&lt;&gt;)</param>
    /// <param name="genericArguments"></param>
    /// <param name="dataArgumentIndex"></param>
    /// <returns>是否是泛型接口的子类型</returns>
    public static bool HasImplementedGenericSpecial(this Type originType, Type genericType, Type[] genericArguments, int dataArgumentIndex)
    {
        //TODO Add test for this
        //继承检查的特化处理逻辑

        while (originType != null && originType != typeof(object))
        {
            if (originType.IsGenericType
                && originType.GetGenericTypeDefinition() is Type originGenericTypeDefinition
                && originGenericTypeDefinition == genericType)
            {
                var originGenericArguments = originType.GetGenericArguments();
                if (originGenericArguments.Length == genericArguments.Length)
                {
                    if (genericArguments.Length == 1)
                    {
                        return true;
                    }
                    bool isMatch = true;
                    for (int i = 0; i < genericArguments.Length; i++)
                    {
                        if (i == dataArgumentIndex)
                        {
                            continue;
                        }
                        if (originGenericArguments[i] != genericArguments[i])
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    if (isMatch)
                    {
                        return true;
                    }
                }
            }
            else if (originType == genericType)
            {
                return true;
            }
            originType = originType.BaseType!;
        }

        return false;
    }

    /// <summary>
    /// 判断 <paramref name="type"/> 是否是泛型 <paramref name="generic"/> 本身，或其直接实现
    /// </summary>
    /// <param name="type"></param>
    /// <param name="generic">typeof(GenericTypeName&lt;&gt;)</param>
    /// <returns></returns>
    public static bool IsTheRawGenericType(this Type type, Type generic) => generic == (type.IsGenericType ? type.GetGenericTypeDefinition() : type);

    /// <summary>
    /// 解包出类型的 <see cref="Task{TResult}"/> 或 <see cref="ValueTask{TResult}"/> 泛型参数 TResult<para/>
    /// 如果 <paramref name="type"/> 非 <see cref="Task{TResult}"/> 或其子类，也非 <see cref="ValueTask{TResult}"/> 实现，则返回自身
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type UnwrapTaskResult(this Type type)
    {
        if (type.IsValueType)   //值类型
        {
            //获取ValueTask的返回类型
            if (type == typeof(ValueTask))
            {
                return typeof(void);
            }
            else if (type.IsGenericType
                     && type.GetGenericTypeDefinition() == typeof(ValueTask<>))
            {
                return type.GetGenericArguments()[0];
            }
        }
        else    //引用类型
        {
            //获取Task的返回类型
            if (type.FindImplementedGenericClassArguments(typeof(Task<>)) is Type[] taskResultTypes
                && taskResultTypes.Length == 1)
            {
                return taskResultTypes[0];
            }
            else if (type.IsAssignableTo(typeof(Task)))
            {
                return typeof(void);
            }
        }

        return type;
    }

    #endregion Public 方法
}
