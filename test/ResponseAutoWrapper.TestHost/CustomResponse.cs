using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

using Cuture.AspNetCore.ResponseAutoWrapper;

namespace ResponseAutoWrapper.TestHost;

public class CustomResponse
{
    #region Public 属性

    public ResponseCode Code { get; set; } = ResponseCode.Success;

    public object Data { get; set; }

    public ResponseMessage Message { get; set; }

    #endregion Public 属性
}

public class CustomResponse<TData>
{
    #region Public 属性

    public ResponseCode Code { get; set; } = ResponseCode.Success;

    public TData Data { get; set; }

    public ResponseMessage Message { get; set; }

    #endregion Public 属性
}

#region Code

public enum ResponseState
{
    Success,
    Fail,
    Error,
}

[JsonConverter(typeof(ResponseCodeConverter))]
public struct ResponseCode
{
    #region Public 字段

    public static readonly ResponseCode Success = new(ResponseState.Success, 0);

    #endregion Public 字段

    #region Public 属性

    public int BusinessCode { get; }

    public ResponseState State { get; }

    #endregion Public 属性

    #region Public 构造函数

    static ResponseCode()
    {

    }

    #endregion Public 构造函数

    #region Private 类

    #endregion Private 类

    #region Public 构造函数

    public ResponseCode(ResponseState state, int businessCode)
    {
        State = state;
        BusinessCode = businessCode;
    }

    #endregion Public 构造函数

    private class ResponseCodeConverter : JsonConverter<ResponseCode>
    {
        #region Private 字段

        private static readonly Dictionary<ResponseState, byte> s_codePrefixMap = new()
        {
            { ResponseState.Success, (byte)'S' },
            { ResponseState.Fail, (byte)'F' },
            { ResponseState.Error, (byte)'E' },
        };

        private static readonly Dictionary<char, ResponseState> s_codePrefixReverseMap = new()
        {
            { 'S',ResponseState.Success },
            { 'F',ResponseState.Fail },
            { 'E',ResponseState.Error },
        };

        #endregion Private 字段

        #region Public 方法

        public override ResponseCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.GetString() is string stringValue)
            {
                var span = stringValue.AsSpan();
                return new ResponseCode(s_codePrefixReverseMap[span[0]], int.Parse(span.Slice(1)));
            }
            return new ResponseCode();
        }

        public override void Write(Utf8JsonWriter writer, ResponseCode value, JsonSerializerOptions options)
        {
            Span<byte> spanValue = stackalloc byte[10];
            spanValue[0] = s_codePrefixMap[value.State];
            var index = Encoding.UTF8.GetBytes(value.BusinessCode.ToString(), spanValue.Slice(1));
            writer.WriteStringValue(spanValue.Slice(0, index + 1));
        }

        #endregion Public 方法
    }
}
#endregion

#region Message

public class ResponseMessage
{
    #region Public 字段

    public static readonly ResponseMessage Success = new() { Text = "SUCCESS" };

    #endregion Public 字段

    #region Public 属性

    public string Text { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ActivityTraceId? TraceId { get; set; }

    #endregion Public 属性

}

#endregion