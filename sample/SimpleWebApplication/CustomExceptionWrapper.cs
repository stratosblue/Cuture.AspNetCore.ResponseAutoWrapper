using System;

using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimpleWebApplication;

// 自定义 Exception 包装器

public class CustomExceptionWrapper : IExceptionWrapper<GenericApiResponse<int, string, object>, int, string>
{
    public GenericApiResponse<int, string, object>? Wrap(HttpContext context, Exception exception)
    {
        return new GenericApiResponse<int, string, object>(13579)
        {
            Message = exception.StackTrace,
        };
    }
}
