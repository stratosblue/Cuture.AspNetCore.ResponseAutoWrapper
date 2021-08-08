﻿using System;

using Cuture.AspNetCore.ResponseAutoWrapper;

namespace ResponseAutoWrapper.TestHost
{
    public class NotGenericCustomResponseCreator : IResponseCreator<CustomResponse>
    {
        public CustomResponse Create(int code, string? message = null)
        {
            return new CustomResponse()
            {
                StatusCode = code,
                Info = message,
            };
        }

        public CustomResponse Create(int code, object? data, string? message = null)
        {
            return new CustomResponse()
            {
                StatusCode = code,
                Datas = data,
                Info = message,
            };
        }

        public CustomResponse Create(int code, Exception exception, string? message = null)
        {
            return new CustomResponse()
            {
                StatusCode = code,
                Info = message,
                ErrorInfo = exception?.StackTrace,
            };
        }

        public object CreateObject(int code, string? message = null, Exception? exception = null)
        {
            return Create(code, exception, message);
        }
    }

    public class CustomResponseCreator : IResponseCreator<CustomResponse<object>>
    {
        public CustomResponse<object> Create(int code, string? message = null)
        {
            return new CustomResponse<object>()
            {
                StatusCode = code,
                Info = message,
            };
        }

        public CustomResponse<object> Create(int code, object? data, string? message = null)
        {
            return new CustomResponse<object>()
            {
                StatusCode = code,
                Datas = data,
                Info = message,
            };
        }

        public CustomResponse<object> Create(int code, Exception exception, string? message = null)
        {
            return new CustomResponse<object>()
            {
                StatusCode = code,
                Info = message,
                ErrorInfo = exception?.StackTrace,
            };
        }

        public object CreateObject(int code, string? message = null, Exception? exception = null)
        {
            return Create(code, exception, message);
        }
    }
}
