using Cuture.AspNetCore.ResponseAutoWrapper;

namespace ResponseAutoWrapper.TestHost;

public class LegacyCustomResponse
{
    public int StatusCode { get; set; } = 200;

    public object Datas { get; set; }

    public string Info { get; set; }
}

public class LegacyCustomResponse<[ResponseData]TData>
{
    public int StatusCode { get; set; } = 200;

    public TData Datas { get; set; }

    public string Info { get; set; }
}
