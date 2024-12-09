using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost;

public class InheritedCustomResponse : CustomResponse<WeatherForecast[]>
{
}

public class InheritedResponse : GenericApiResponse<ResponseCode, ResponseMessage, WeatherForecast[]>
{
    #region Public 构造函数

    public InheritedResponse() : base(ResponseCode.Success)
    {
    }

    #endregion Public 构造函数
}

public class InheritedCustomResponseNotGeneric : CustomResponse
{
}
