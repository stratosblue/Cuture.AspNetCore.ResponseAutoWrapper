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
        /// 是否将捕获到的异常抛出给上层中间件<para/>
        /// default is "fasle"
        /// </summary>
        public bool ThrowCaughtExceptions { get; set; } = false;

        #endregion Public 属性
    }
}