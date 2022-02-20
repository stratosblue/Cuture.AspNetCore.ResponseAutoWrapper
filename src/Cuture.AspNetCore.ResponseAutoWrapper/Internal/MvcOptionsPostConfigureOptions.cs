using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal;

internal class MvcOptionsPostConfigureOptions : PostConfigureOptions<MvcOptions>
{
    #region Public 构造函数

    public MvcOptionsPostConfigureOptions(string name, Action<MvcOptions> action) : base(name, action)
    {
    }

    #endregion Public 构造函数
}
