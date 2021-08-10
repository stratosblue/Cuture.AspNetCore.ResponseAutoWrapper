namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 有类型的响应创建器
    /// </summary>
    /// <typeparam name="TResponse">响应类型</typeparam>
    public interface IResponseCreator<TResponse>
        where TResponse : class
    {
        #region Public 方法

        /// <inheritdoc cref="Create(int, object?, string?)"/>
        TResponse Create(int code, string? message = null);

        /// <summary>
        /// 创建响应对象
        /// </summary>
        /// <param name="code">响应code</param>
        /// <param name="data">响应数据</param>
        /// <param name="message">响应消息</param>
        /// <returns></returns>
        TResponse Create(int code, object? data, string? message = null);

        #endregion Public 方法
    }
}