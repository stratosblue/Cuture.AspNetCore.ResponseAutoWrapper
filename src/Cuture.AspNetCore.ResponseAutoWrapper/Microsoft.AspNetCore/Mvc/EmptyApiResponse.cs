namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// 空API响应
    /// </summary>
    public class EmptyApiResponse : ApiResponse<object>
    {
        #region Public 方法

        /// <summary>
        /// 创建一个响应
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static EmptyApiResponse Create(int code) => new() { Code = code };

        /// <summary>
        /// 创建一个响应
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static EmptyApiResponse Create(int code, string message) => new() { Code = code, Message = message };

        /// <summary>
        /// 创建一个响应
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static EmptyApiResponse Create(string message) => new() { Message = message };

        #endregion Public 方法
    }
}