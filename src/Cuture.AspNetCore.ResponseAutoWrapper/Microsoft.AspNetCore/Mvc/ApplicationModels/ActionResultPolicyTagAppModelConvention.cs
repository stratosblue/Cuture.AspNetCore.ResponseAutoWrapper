using System;
using System.Linq;
using System.Reflection;

using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.ApplicationModels
{
    /// <summary>
    /// 用于标记ActionResult处理策略的ApplicationModelConvention
    /// </summary>
    internal class ActionResultPolicyTagAppModelConvention : IApplicationModelConvention
    {
        #region Private 字段

        private readonly Func<MemberInfo, bool> _actionNoWrapPredicate;

        #endregion Private 字段

        #region Protected 属性

        protected IWrapTypeCreator WrapTypeCreator { get; }

        #endregion Protected 属性

        #region Public 构造函数

        public ActionResultPolicyTagAppModelConvention(IWrapTypeCreator wrapTypeCreator, Func<MemberInfo, bool> actionNoWrapPredicate)
        {
            WrapTypeCreator = wrapTypeCreator ?? throw new ArgumentNullException(nameof(wrapTypeCreator));
            _actionNoWrapPredicate = actionNoWrapPredicate ?? throw new ArgumentNullException(nameof(actionNoWrapPredicate));
        }

        #endregion Public 构造函数

        #region Public 方法

        public void Apply(ApplicationModel application)
        {
            var actions = application.Controllers.SelectMany(m => m.Actions);
            foreach (var action in actions)
            {
                ProcessActionModel(action);
            }
        }

        #endregion Public 方法

        #region Protected 方法

        /// <summary>
        /// 在Action需要包装时触发的函数
        /// </summary>
        /// <param name="action">Action</param>
        /// <param name="returnType">实际返回数据类型</param>
        protected virtual void OnActionShouldWrap(ActionModel action, Type returnType)
        {
        }

        #endregion Protected 方法

        #region Private 方法

        private void ProcessActionModel(ActionModel action)
        {
            var returnType = action.ActionMethod.ReturnType.UnwrapTaskResult();

            if (_actionNoWrapPredicate(action.ActionMethod)
                || _actionNoWrapPredicate(action.Controller.ControllerType)
                || !WrapTypeCreator.ShouldWrap(returnType))
            {
                SetActionResultPolicy(ActionResultPolicy.Skip);
                return;
            }

            SetActionResultPolicy(returnType == typeof(object) ? ActionResultPolicy.Unknown : ActionResultPolicy.Process);

            OnActionShouldWrap(action, returnType);

            void SetActionResultPolicy(ActionResultPolicy policy)
            {
                action.Properties[Constants.ActionPropertiesResultPolicyKey] = policy;
            }
        }

        #endregion Private 方法
    }
}