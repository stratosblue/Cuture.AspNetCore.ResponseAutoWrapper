﻿using System;
using System.Linq;
using System.Reflection;

using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.AspNetCore.Mvc.ApplicationModels;

/// <summary>
/// 用于标记ActionResult处理策略的ApplicationModelConvention
/// </summary>
internal class ActionResultPolicyTagAppModelConvention<TCode, TMessage> : IApplicationModelConvention
{
    #region Private 字段

    private readonly Func<MemberInfo, bool> _actionNoWrapPredicate;

    #endregion Private 字段

    #region Protected 属性

    protected IWrapTypeCreator<TCode, TMessage> WrapTypeCreator { get; }

    #endregion Protected 属性

    #region Public 构造函数

    public ActionResultPolicyTagAppModelConvention(IWrapTypeCreator<TCode, TMessage> wrapTypeCreator, Func<MemberInfo, bool> actionNoWrapPredicate)
    {
        ArgumentNullException.ThrowIfNull(wrapTypeCreator);
        ArgumentNullException.ThrowIfNull(actionNoWrapPredicate);

        WrapTypeCreator = wrapTypeCreator;
        _actionNoWrapPredicate = actionNoWrapPredicate;
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
    /// 设置Action的Result处理策略
    /// </summary>
    /// <param name="action"></param>
    /// <param name="policy"></param>
    protected static void SetActionResultPolicy(ActionModel action, ActionResultPolicy policy)
    {
        action.Properties[Constants.ActionPropertiesResultPolicyKey] = policy;
    }

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
            SetActionResultPolicy(action, ActionResultPolicy.Skip);
            return;
        }

        SetActionResultPolicy(action, returnType == typeof(object) ? ActionResultPolicy.Unknown : ActionResultPolicy.Process);

        OnActionShouldWrap(action, returnType);
    }

    #endregion Private 方法
}
