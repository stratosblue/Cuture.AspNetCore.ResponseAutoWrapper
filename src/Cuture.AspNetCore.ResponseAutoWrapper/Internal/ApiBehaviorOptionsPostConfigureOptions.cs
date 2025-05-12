using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal;

internal class ApiBehaviorOptionsPostConfigureOptions(string name, Action<ApiBehaviorOptions> action)
    : PostConfigureOptions<ApiBehaviorOptions>(name, action)
{
}
