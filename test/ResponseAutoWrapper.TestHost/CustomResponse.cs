using System;

using Cuture.AspNetCore.ResponseAutoWrapper;

namespace ResponseAutoWrapper.TestHost
{
    public class CustomResponse
    {
        public int StatusCode { get; set; } = 200;

        public object Datas { get; set; }

        public string Info { get; set; }

        public string ErrorInfo { get; set; }
    }

    public class CustomResponse<TData>
    {
        public int StatusCode { get; set; } = 200;

        public TData Datas { get; set; }

        public string Info { get; set; }

        public string ErrorInfo { get; set; }
    }

    public class CustomResponseI : ISetResponseCode, ISetResponseData, ISetResponseException, ISetResponseMessage
    {
        public int ResultCode { get; set; } = 200;

        public object Result { get; set; }

        public string Msg { get; set; }

        public string Error { get; set; }

        public void SetCode(int code) => ResultCode = code;

        public void SetData(object? data) => Result = data;

        public void SetException(Exception? exception)
        {
            if (exception is not null)
            {
                ResultCode = 500;
                Msg = exception.Message;
                Error = exception.StackTrace;
            }
        }

        public void SetMessage(string? message) => Msg = message;
    }

    public class CustomResponseI<TData> : ISetResponseCode, ISetResponseData, ISetResponseException, ISetResponseMessage
    {
        public int ResultCode { get; set; } = 200;

        public TData Result { get; set; }

        public string Msg { get; set; }

        public string Error { get; set; }

        public void SetCode(int code) => ResultCode = code;

        public void SetData(object? data) => Result = (TData)data;

        public void SetException(Exception? exception)
        {
            if (exception is not null)
            {
                ResultCode = 500;
                Msg = exception.Message;
                Error = exception.StackTrace;
            }
        }

        public void SetMessage(string? message) => Msg = message;
    }
}
