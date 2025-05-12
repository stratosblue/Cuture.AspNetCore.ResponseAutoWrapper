using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal;

internal class MvcOptionsPostConfigureOptions(string name, Action<MvcOptions> action)
    : PostConfigureOptions<MvcOptions>(name, action)
{
}
