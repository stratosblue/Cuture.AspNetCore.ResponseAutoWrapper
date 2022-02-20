using System;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 不进行响应包装
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class NoResponseWrapAttribute : Attribute
{
}
