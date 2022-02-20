using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost;

public class InheritedApiResponse : ApiResponse<WeatherForecast[]>
{
}

public class InheritedCustomResponse : CustomResponse<WeatherForecast[]>
{
}

public class InheritedCustomResponseI : CustomResponseI<WeatherForecast[]>
{
}

public class InheritedCustomResponseNotGeneric : CustomResponse
{
}

public class InheritedCustomResponseINotGeneric : CustomResponseI
{
}
