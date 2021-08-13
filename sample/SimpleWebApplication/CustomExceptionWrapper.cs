using System;

using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimpleWebApplication
{
    // 自定义 Exception 包装器

    public class CustomExceptionWrapper : IExceptionWrapper<ApiResponse<object>>
    {
        public ApiResponse<object>? Wrap(HttpContext context, Exception exception)
        {
            return new ApiResponse<object>()
            {
                Code = 13579,
                Message = exception.StackTrace,
            };
        }
    }
}