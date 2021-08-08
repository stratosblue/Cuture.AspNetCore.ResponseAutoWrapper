using System;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal
{
    /// <summary>
    /// 默认的无效模型响应格式化器
    /// </summary>
    internal class DefaultInvalidModelStateResponseFormatter<TResponse> : IInvalidModelStateResponseFormatter<TResponse>
        where TResponse : notnull
    {
        #region Private 字段

        private readonly IResponseCreator<TResponse> _responseCreator;

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="DefaultInvalidModelStateResponseFormatter{TResponse}"/>
        public DefaultInvalidModelStateResponseFormatter(IResponseCreator<TResponse> responseCreator)
        {
            _responseCreator = responseCreator;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public TResponse Handle(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;

            var errorMessages = context.ModelState.Where(m => m.Value.Errors.Count > 0)
                                                  .Select(m => $"{m.Key} - {m.Value.Errors.FirstOrDefault()?.ErrorMessage}");

            var message = string.Join(Environment.NewLine, errorMessages);

            return _responseCreator.Create(StatusCodes.Status400BadRequest, message: message);
        }

        #endregion Public 方法
    }
}