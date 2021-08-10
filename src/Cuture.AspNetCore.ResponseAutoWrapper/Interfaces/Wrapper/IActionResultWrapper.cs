using Microsoft.AspNetCore.Mvc.Filters;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// <see cref="IAsyncResultFilter"/> 中用以包装 ActionResult 的包装器
    /// </summary>
    /// <typeparam name="TResponse">统一响应类型</typeparam>
    public interface IActionResultWrapper<TResponse> : IWrapper<TResponse> where TResponse : class
    {
        #region Public 方法

        /// <summary>
        /// 通过 <see cref="ResultExecutingContext"/> 返回包装后的统一响应
        /// </summary>
        /// <param name="context"></param>
        /// <returns>包装后的统一响应类型对象，返回 null 时，不做处理</returns>
        TResponse? Wrap(ResultExecutingContext context);

        #endregion Public 方法
    }
}