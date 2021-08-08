using System;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 设置响应异常接口<para/>
    /// 当需要替换默认响应类型，且不自行实现<see cref="IResponseCreator{TResponse}"/>时，实现此接口以处理响应异常
    /// </summary>
    public interface ISetResponseException
    {
        #region Public 方法

        /// <summary>
        /// 设置响应异常
        /// </summary>
        /// <param name="exception"></param>
        void SetException(Exception? exception);

        #endregion Public 方法
    }
}