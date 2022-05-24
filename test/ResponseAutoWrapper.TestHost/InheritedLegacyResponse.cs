using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost;

public class InheritedLegacyResponse : ApiResponse<WeatherForecast[]>
{
    public InheritedLegacyResponse() : base(200)
    {
    }
}

public class InheritedLegacyCustomResponse : LegacyCustomResponse<WeatherForecast[]>
{
}

public class InheritedLegacyCustomResponseNotGeneric : LegacyCustomResponse
{
}
