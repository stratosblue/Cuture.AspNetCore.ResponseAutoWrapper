using System;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 默认的 <inheritdoc cref="IInvalidModelStateWrapper{TResponse}"/>
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class DefaultInvalidModelStateWrapper<TResponse> : IInvalidModelStateWrapper<TResponse>
        where TResponse : class
    {
        #region Private 字段

        private readonly IResponseCreator<TResponse> _responseCreator;

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="DefaultInvalidModelStateWrapper{TResponse}"/>
        public DefaultInvalidModelStateWrapper(IResponseCreator<TResponse> responseCreator)
        {
            _responseCreator = responseCreator ?? throw new ArgumentNullException(nameof(responseCreator));
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public TResponse Wrap(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;

            var errorMessages = context.ModelState.Where(m => m.Value?.Errors.Count > 0)
                                                  .Select(m => $"{m.Key} - {m.Value?.Errors.FirstOrDefault()?.ErrorMessage}");

            var message = string.Join(Environment.NewLine, errorMessages);

            return _responseCreator.Create(code: StatusCodes.Status400BadRequest, message: message);
        }

        #endregion Public 方法
    }
}