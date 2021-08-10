namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 响应描述
    /// </summary>
    public class ResponseDescription
    {
        /// <summary>
        /// <see cref="Code"/> 为 200 的空描述
        /// </summary>
        public static readonly ResponseDescription Empty = new(200);

        /// <summary>
        /// Code
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// 消息
        /// </summary>
        public string? Message { get; }

        /// <inheritdoc cref="ResponseDescription(int, string?)"/>
        public ResponseDescription(int code)
        {
            Code = code;
        }

        /// <inheritdoc cref="ResponseDescription(int, string?)"/>
        public ResponseDescription(string? message)
        {
            Code = 200;
            Message = message;
        }

        /// <summary>
        /// <inheritdoc cref="ResponseDescription"/>
        /// </summary>
        /// <param name="code">Code</param>
        /// <param name="message">消息</param>
        public ResponseDescription(int code, string? message)
        {
            Code = code;
            Message = message;
        }

        /// <inheritdoc/>
        public override string ToString() => $"Code: {Code} , Message: {Message}";
    }
}