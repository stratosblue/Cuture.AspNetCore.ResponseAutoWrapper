using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc;

/// <summary>
/// 空API响应
/// </summary>
public class EmptyApiResponse(int code) : ApiResponse(code)
{

    #region Public 方法

    /// <summary>
    /// 创建一个响应
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static EmptyApiResponse Create(int code) => new(code);

    /// <summary>
    /// 创建一个响应
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static EmptyApiResponse Create(int code, string message) => new(code) { Message = message };

    /// <summary>
    /// 创建一个响应
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static EmptyApiResponse Create(string message) => new(StatusCodes.Status200OK) { Message = message };

    #endregion Public 方法
}
