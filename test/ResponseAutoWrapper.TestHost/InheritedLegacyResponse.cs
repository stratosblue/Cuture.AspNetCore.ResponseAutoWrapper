using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost;

public class InheritedLegacyCustomResponse : LegacyCustomResponse<WeatherForecast[]>
{
}

public class InheritedLegacyResponse : ApiResponse<WeatherForecast[]>
{
    #region Public 构造函数

    public InheritedLegacyResponse() : base(200)
    {
    }

    #endregion Public 构造函数
}

public class InheritedLegacyCustomResponseNotGeneric : LegacyCustomResponse
{
}
