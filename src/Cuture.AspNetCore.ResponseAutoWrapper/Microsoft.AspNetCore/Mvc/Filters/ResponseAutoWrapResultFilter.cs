using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.Filters
{
    internal class ResponseAutoWrapResultFilter<TResponse> : IAsyncAlwaysRunResultFilter
        where TResponse : notnull
    {
        #region Private 字段

        private readonly ILogger _logger;

        private readonly IResponseCreator<TResponse> _responseCreator;

        private readonly IWrapTypeCreator _wrapTypeCreator;

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="ResponseAutoWrapResultFilter{TResponse}"/>
        public ResponseAutoWrapResultFilter(IResponseCreator<TResponse> responseCreator,
                                            IWrapTypeCreator wrapTypeCreator,
                                            ILogger<ResponseAutoWrapResultFilter<TResponse>> logger)
        {
            _responseCreator = responseCreator ?? throw new ArgumentNullException(nameof(responseCreator));
            _wrapTypeCreator = wrapTypeCreator ?? throw new ArgumentNullException(nameof(wrapTypeCreator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            switch (GetActionResultPolicy(context))
            {
                case ActionResultPolicy.Process:
                    {
                        ProcessResult(_ => true);
                    }
                    break;

                case ActionResultPolicy.Skip:
                    break;

                case ActionResultPolicy.Unknown:
                default:
                    {
                        bool ProcessPredicate(ObjectResult objectResult)
                            => _wrapTypeCreator.ShouldWrap(objectResult.Value.GetType());

                        ProcessResult(ProcessPredicate);
                    }
                    break;
            }

            await next();

            void ProcessResult(Func<ObjectResult, bool> processPredicate)
            {
                if (context.Result is ObjectResult objectResult)
                {
                    if (objectResult.Value is not TResponse
                        && (objectResult.Value is null
                            || processPredicate(objectResult)))
                    {
                        var response = _responseCreator.Create(StatusCodes.Status200OK, objectResult.Value, default);

                        context.Result = new OkObjectResult(response)
                        {
                            DeclaredType = typeof(TResponse),
                            Formatters = objectResult.Formatters,
                            ContentTypes = objectResult.ContentTypes,
                        };
                    }
                }
                else if (context.Result is EmptyResult
                         || context.Result is null)
                {
                    context.Result = new OkObjectResult(_responseCreator.Create(StatusCodes.Status200OK));
                }
                else
                {
                    _logger.LogError("暂时无法处理 {0} 的返回值，类型 - {1} : {2}", context.ActionDescriptor.DisplayName, context.Result.GetType(), context.Result);

                    var response = _responseCreator.Create(StatusCodes.Status500InternalServerError, message: $"Can not wrap result - {context.Result} of {context.ActionDescriptor.DisplayName}");

                    context.Result = new OkObjectResult(response);
                }
            }
        }

        #endregion Public 方法

        #region Private 方法

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ActionResultPolicy GetActionResultPolicy(ResultExecutingContext context)
        {
            //HACK 不加锁读取 ActionDescriptor.Properties ，可能存在某些问题
            if (context.ActionDescriptor.Properties.TryGetValue(Constants.ActionPropertiesResultPolicyKey, out var cachedPolicy))
            {
                return (ActionResultPolicy)cachedPolicy;
            }

            //HACK 理论上不应该执行到这里
            Debug.WriteLine("Warning!!! Not found Constants.ActionPropertiesResultPolicyKey in ActionDescriptor.Properties");

            return ActionResultPolicy.Unknown;
        }

        #endregion Private 方法
    }
}