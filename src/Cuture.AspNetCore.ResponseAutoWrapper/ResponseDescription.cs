namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// 响应描述
/// </summary>
public class ResponseDescription<TCode, TMessage>
{
    #region Public 属性

    /// <summary>
    /// Code
    /// </summary>
    public TCode Code { get; }

    /// <summary>
    /// 消息
    /// </summary>
    public TMessage? Message { get; }

    #endregion Public 属性

    #region Public 构造函数

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

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public override string ToString() => $"Code: {Code} , Message: {Message}";

    #endregion Public 方法
}
