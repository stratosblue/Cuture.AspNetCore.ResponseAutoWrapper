using System;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Formatters;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    ///
    /// </summary>
    public class ResponseAutoWrapMiddlewareOptions
    {
        #region Public 属性

        /// <summary>
        /// 是否捕获异常<para/>
        /// default is "true"
        /// </summary>
        public bool CatchExceptions { get; set; } = true;

        /// <summary>
        /// 默认输出格式化器选择委托<para/>
        /// 选择在请求中无 Accept 时，用于格式化响应的 <see cref="IOutputFormatter"/>
        /// </summary>
        public Func<FormatterCollection<IOutputFormatter>, IOutputFormatter> DefaultOutputFormatterSelector { get; set; }
            = static formatters => formatters.FirstOrDefault(m => m.GetType() == typeof(SystemTextJsonOutputFormatter))
                                                                ?? throw new InvalidOperationException($"Can not found {nameof(SystemTextJsonOutputFormatter)} by default. Must select a formatter manually.");

        /// <summary>
        /// 是否将捕获到的异常抛出给上层中间件<para/>
        /// default is "fasle"
        /// </summary>
        public bool ThrowCaughtExceptions { get; set; } = false;

        #endregion Public 属性
    }
}