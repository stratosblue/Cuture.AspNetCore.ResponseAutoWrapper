using System.Text.Json.Serialization;

namespace CustomStructureWebApplication;

public class CommonResponse<TData>
{
    public string Code { get; set; }

    public RichMessage Message { get; set; }

    public TData Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string TraceId { get; set; }
}

public class RichMessage
{
    public string Content { get; set; }
}
