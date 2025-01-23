using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
///
/// </summary>
public class ResponseAutoWrapMiddlewareOptions
{
    #region Public 委托

    /// <summary>
    /// 中间件异常已捕获
    /// </summary>
    /// <param name="request">出现异常的请求</param>
    /// <param name="exception">异常</param>
    /// <param name="doesExceptionWrapped">异常是否已包装</param>
    public delegate void MiddlewareExceptionCaptured(HttpRequest request, Exception exception, in bool doesExceptionWrapped);

    #endregion Public 委托

    #region Public 属性

    /// <summary>
    /// 是否捕获异常<para/>
    /// default is <see langword="true"/>
    /// </summary>
    public bool CatchExceptions { get; set; } = true;

    /// <summary>
    /// 默认输出格式化器选择委托<para/>
    /// 选择在请求中无 Accept 时，用于格式化响应的 <see cref="IOutputFormatter"/><para/>
    /// 默认时会选择 <see cref="SystemTextJsonOutputFormatter"/> ，不存在则会抛出异常
    /// </summary>
    public Func<FormatterCollection<IOutputFormatter>, IOutputFormatter> DefaultOutputFormatterSelector { get; set; }
        = static formatters => formatters.FirstOrDefault(m => m.GetType() == typeof(SystemTextJsonOutputFormatter))
                                                            ?? throw new InvalidOperationException($"Can not found {nameof(SystemTextJsonOutputFormatter)} by default. Must select a formatter manually by \"{nameof(ResponseAutoWrapMiddlewareOptions)}.{nameof(DefaultOutputFormatterSelector)}\" at middleware setup.");

    /// <summary>
    /// 忽略 OPTIONS 请求
    /// </summary>
    public bool IgnoreOptionsRequest { get; set; } = true;

    /// <summary>
    /// 在中间件已捕获异常时的回调
    /// </summary>
    public MiddlewareExceptionCaptured? OnMiddlewareExceptionCaptured { get; set; }

    /// <summary>
    /// 是否将捕获到的异常抛出给上层中间件<para/>
    /// default is <see langword="false"/>
    /// </summary>
    public bool ThrowCaughtExceptions { get; set; } = false;

    #endregion Public 属性
}
