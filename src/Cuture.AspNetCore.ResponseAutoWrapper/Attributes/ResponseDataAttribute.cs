using System;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
///
/// </summary>
[AttributeUsage(AttributeTargets.GenericParameter | AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
[Obsolete("此特性已移除，现在按约定进行配置，约定Data对应的泛型参数为最后一个泛型参数。", true)]
public sealed class ResponseDataAttribute : Attribute
{
}
