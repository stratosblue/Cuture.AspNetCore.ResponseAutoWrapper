using System;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
///
/// </summary>
[AttributeUsage(AttributeTargets.GenericParameter | AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ResponseDataAttribute : Attribute
{
}
