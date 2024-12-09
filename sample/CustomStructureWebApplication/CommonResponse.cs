using System.Text.Json.Serialization;

namespace CustomStructureWebApplication;

public class CommonResponse<TData>
{
    #region Public 属性

    public string Code { get; set; }

    public TData Data { get; set; }

    public RichMessage Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string TraceId { get; set; }

    #endregion Public 属性
}

public class RichMessage
{
    #region Public 属性

    public string Content { get; set; }

    #endregion Public 属性
}
