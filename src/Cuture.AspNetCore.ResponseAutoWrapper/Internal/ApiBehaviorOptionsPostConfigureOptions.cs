using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal;

internal class ApiBehaviorOptionsPostConfigureOptions : PostConfigureOptions<ApiBehaviorOptions>
{
    #region Public 构造函数

    public ApiBehaviorOptionsPostConfigureOptions(string name, Action<ApiBehaviorOptions> action) : base(name, action)
    {
    }

    #endregion Public 构造函数
}
