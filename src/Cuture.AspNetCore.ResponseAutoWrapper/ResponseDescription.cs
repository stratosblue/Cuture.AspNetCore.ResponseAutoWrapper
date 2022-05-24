namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 响应描述
/// </summary>
public class ResponseDescription<TCode, TMessage>
{
    /// <summary>
    /// Code
    /// </summary>
    public TCode Code { get; }

    /// <summary>
    /// 消息
    /// </summary>
    public TMessage? Message { get; }

    /// <inheritdoc cref="ResponseDescription(TCode, TMessage?)"/>
    public ResponseDescription(TCode code)
    {
        Code = code;
    }

    /// <summary>
    /// <inheritdoc cref="ResponseDescription{TCode, TMessage}"/>
    /// </summary>
    /// <param name="code">Code</param>
    /// <param name="message">消息</param>
    public ResponseDescription(TCode code, TMessage? message)
    {
        Code = code;
        Message = message;
    }

    /// <inheritdoc/>
    public override string ToString() => $"Code: {Code} , Message: {Message}";
}
