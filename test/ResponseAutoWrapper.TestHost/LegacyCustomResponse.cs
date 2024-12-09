namespace ResponseAutoWrapper.TestHost;

public class LegacyCustomResponse
{
    #region Public 属性

    public object Datas { get; set; }

    public string Info { get; set; }

    public int StatusCode { get; set; } = 200;

    #endregion Public 属性
}

public class LegacyCustomResponse<TData>
{
    #region Public 属性

    public TData Datas { get; set; }

    public string Info { get; set; }

    public int StatusCode { get; set; } = 200;

    #endregion Public 属性
}
