using System;
using System.Reflection;

using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.ApplicationModels
{
    internal class OpenAPISupportAppModelConvention : ActionResultPolicyTagAppModelConvention
    {
        #region Public 构造函数

        public OpenAPISupportAppModelConvention(IWrapTypeCreator wrapTypeCreator, Func<MemberInfo, bool> actionNoWrapPredicate)
            : base(wrapTypeCreator, actionNoWrapPredicate)
        {
        }

        #endregion Public 构造函数

        #region Protected 方法

        /// <inheritdoc/>
        protected override void OnActionShouldWrap(ActionModel action, Type returnType)
        {
            base.OnActionShouldWrap(action, returnType);

            var redirectReturnType = returnType == typeof(void)
                                         ? WrapTypeCreator.ResponseType
                                         : WrapTypeCreator.MakeWrapType(returnType);

            action.Filters.Add(new ProducesResponseTypeAttribute(redirectReturnType, StatusCodes.Status200OK));
        }

        #endregion Protected 方法
    }
}