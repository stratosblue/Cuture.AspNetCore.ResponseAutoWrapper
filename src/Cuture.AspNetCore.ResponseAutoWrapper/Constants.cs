using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Cuture.AspNetCore.ResponseAutoWrapper;

/// <summary>
/// ResponseAutoWrapper 相关常量
/// </summary>
public static class Constants
{
    #region Public 字段

    /// <summary>
    /// <see cref="ActionModel.Properties"/> 中存放 ActionResult 处理策略的Key
    /// </summary>
    public const string ActionPropertiesResultPolicyKey = "ACTION_PROPERTIES_RESULT_POLICY_KEY";

    /// <summary>
    /// 默认的 FilterOrder
    /// </summary>
    public const int DefaultFilterOrder = -5000;

    /// <summary>
    /// <see cref="HttpContext.Items"/> 中存放不包装响应内容的标记的Key
    /// </summary>
    public const string HttpContextDoNotWrapMarkKey = "HTTPCONTEXT_DO_NOT_WRAP_MARK_KEY";

    #endregion Public 字段
}
