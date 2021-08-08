namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 设置响应码接口<para/>
    /// 当需要替换默认响应类型，且不自行实现<see cref="IResponseCreator{TResponse}"/>时，实现此接口以处理响应码
    /// </summary>
    public interface ISetResponseCode
    {
        #region Public 方法

        /// <summary>
        /// 设置响应码
        /// </summary>
        /// <param name="code"></param>
        void SetCode(int code);

        #endregion Public 方法
    }
}