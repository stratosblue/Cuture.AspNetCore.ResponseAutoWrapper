namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 设置响应消息接口<para/>
    /// 当需要替换默认响应类型，且不自行实现<see cref="IResponseCreator{TResponse}"/>时，实现此接口以处理响应消息
    /// </summary>
    public interface ISetResponseMessageFeature
    {
        #region Public 方法

        /// <summary>
        /// 设置响应消息
        /// </summary>
        /// <param name="message"></param>
        void SetMessage(string? message);

        #endregion Public 方法
    }
}