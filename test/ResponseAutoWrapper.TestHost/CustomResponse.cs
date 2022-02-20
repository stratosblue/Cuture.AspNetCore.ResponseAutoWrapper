using System;

using Cuture.AspNetCore.ResponseAutoWrapper;

namespace ResponseAutoWrapper.TestHost;

public class CustomResponse
{
    public int StatusCode { get; set; } = 200;

    public object Datas { get; set; }

    public string Info { get; set; }
}

public class CustomResponse<TData>
{
    public int StatusCode { get; set; } = 200;

    public TData Datas { get; set; }

    public string Info { get; set; }
}

public class CustomResponseI : ISetResponseCodeFeature, ISetResponseDataFeature, ISetResponseMessageFeature
{
    public int ResultCode { get; set; } = 200;

    public object Result { get; set; }

    public string Msg { get; set; }

    public void SetCode(int code) => ResultCode = code;

    public void SetData(object? data) => Result = data;

    public void SetMessage(string? message) => Msg = message;
}

public class CustomResponseI<TData> : ISetResponseCodeFeature, ISetResponseDataFeature, ISetResponseMessageFeature
{
    public int ResultCode { get; set; } = 200;

    public TData Result { get; set; }

    public string Msg { get; set; }

    public void SetCode(int code) => ResultCode = code;

    public void SetData(object? data) => Result = (TData)data;

    public void SetMessage(string? message) => Msg = message;
}
