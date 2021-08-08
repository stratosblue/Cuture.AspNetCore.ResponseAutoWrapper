using System.Threading.Tasks;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 中间件状态码处理筛选委托<para/>
    /// 委托返回 true 时，表明此请求的响应需要被包装<para/>
    /// </summary>
    public delegate ValueTask<bool> MiddlewareStatusCodePredicate(int statusCode);

    /// <summary>
    ///
    /// </summary>
    public class ResponseAutoWrapMiddlewareOptions
    {
        #region Private 字段

        /// <inheritdoc cref="MiddlewareStatusCodePredicate"/>
        private MiddlewareStatusCodePredicate? _middlewareStatusCodePredicate;

        #endregion Private 字段

        #region Public 属性

        /// <summary>
        /// 是否捕获异常<para/>
        /// default is "true"
        /// </summary>
        public bool CatchExceptions { get; set; } = true;

        /// <inheritdoc cref="ResponseAutoWrapper.MiddlewareStatusCodePredicate"/>
        public MiddlewareStatusCodePredicate MiddlewareStatusCodePredicate { get => _middlewareStatusCodePredicate ?? DefaultMiddlewareStatusCodeCheck; set => _middlewareStatusCodePredicate = value; }

        /// <summary>
        /// 是否将捕获到的异常抛出给上层中间件<para/>
        /// default is "fasle"
        /// </summary>
        public bool ThrowCaughtExceptions { get; set; } = false;

        #endregion Public 属性

        #region Public 方法

        /// <summary>
        /// 默认的 中间件状态码处理筛选委托<para/>
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static ValueTask<bool> DefaultMiddlewareStatusCodeCheck(int statusCode)
            => new(statusCode is < 300 or > 399);

        #endregion Public 方法
    }
}