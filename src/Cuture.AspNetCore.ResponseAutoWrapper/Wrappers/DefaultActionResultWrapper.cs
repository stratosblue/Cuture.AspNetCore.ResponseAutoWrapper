using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Cuture.AspNetCore.ResponseAutoWrapper
{
    /// <summary>
    /// 默认的 <inheritdoc cref="IActionResultWrapper{TResponse}"/>
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class DefaultActionResultWrapper<TResponse> : IActionResultWrapper<TResponse>
        where TResponse : class
    {
        #region Private 字段

        private readonly ILogger _logger;
        private readonly IResponseCreator<TResponse> _responseCreator;
        private readonly IWrapTypeCreator _wrapTypeCreator;

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="DefaultActionResultWrapper{TResponse}"/>
        public DefaultActionResultWrapper(IResponseCreator<TResponse> responseCreator,
                                          IWrapTypeCreator wrapTypeCreator,
                                          ILogger<DefaultActionResultWrapper<TResponse>> logger)
        {
            _responseCreator = responseCreator ?? throw new ArgumentNullException(nameof(responseCreator));
            _wrapTypeCreator = wrapTypeCreator ?? throw new ArgumentNullException(nameof(wrapTypeCreator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public TResponse? Wrap(ResultExecutingContext context)
        {
            var actionResult = context.Result;
            if (actionResult is ObjectResult objectResult)
            {
                if (objectResult.Value is not TResponse
                    && (objectResult.Value is null
                        || _wrapTypeCreator.ShouldWrap(objectResult.Value.GetType())))
                {
                    var description = context.HttpContext.GetResponseDescription();
                    return _responseCreator.Create(description.Code, objectResult.Value, description.Message);
                }
                return null;
            }
            else if (actionResult is EmptyResult
                     || actionResult is null)
            {
                return _responseCreator.Create(StatusCodes.Status200OK);
            }

            _logger.LogError("暂时无法处理的返回值类型 - {1} : {2}", actionResult.GetType(), actionResult);

            return null;
        }

        #endregion Public 方法
    }
}