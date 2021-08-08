namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 设置响应数据接口<para/>
    /// 当需要替换默认响应类型，且不自行实现<see cref="IResponseCreator{TResponse}"/>时，实现此接口以处理响应数据
    /// </summary>
    public interface ISetResponseData
    {
        #region Public 方法

        /// <summary>
        /// 设置响应数据
        /// </summary>
        /// <param name="data"></param>
        void SetData(object? data);

        #endregion Public 方法
    }
}