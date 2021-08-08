using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 无效模型状态响应格式化器
    /// </summary>
    public interface IInvalidModelStateResponseFormatter<TResponse>
    {
        #region Public 方法

        /// <summary>
        /// 格式化模型异常为响应格式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        TResponse? Handle(ActionContext context);

        #endregion Public 方法
    }
}