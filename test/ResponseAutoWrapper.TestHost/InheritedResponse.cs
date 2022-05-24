using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost;

public class InheritedResponse : GenericApiResponse<ResponseCode, ResponseMessage, WeatherForecast[]>
{
    public InheritedResponse() : base(ResponseCode.Success)
    {
    }
}

public class InheritedCustomResponse : CustomResponse<WeatherForecast[]>
{
}

public class InheritedCustomResponseNotGeneric : CustomResponse
{
}
