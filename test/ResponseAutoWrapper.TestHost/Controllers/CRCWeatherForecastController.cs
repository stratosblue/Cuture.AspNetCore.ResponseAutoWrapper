﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace ResponseAutoWrapper.TestHost.Controllers
{
    //CustomResponseByResponseCreator
    //Copy from CRIWeatherForecastController

    [ApiController]
    [Route("api/[controller]")]
    public class CRCWeatherForecastController : ControllerBase
    {
        #region Public 方法

        [HttpGet]
        [Route("get")]
        public WeatherForecast[] Get()
        {
            return WeatherForecast.GenerateData();
        }

        [HttpGet]
        [Route("get-inheritedtask")]
        public InheritedTask<WeatherForecast[]> GetByInheritedTask()
        {
            var task = new InheritedTask<WeatherForecast[]>(() => WeatherForecast.GenerateData());
            task.Start();
            return task;
        }

        [HttpGet]
        [Route("get-inheritedtask2")]
        public TwiceInheritedTask GetByInheritedTask2()
        {
            var task = new TwiceInheritedTask(() => WeatherForecast.GenerateData());
            task.Start();
            return task;
        }

        [HttpGet]
        [Route("get-direct-api-response")]
        public virtual CustomResponse<WeatherForecast[]> GetDirectApiResponse()
        {
            return new CustomResponse<WeatherForecast[]>() { Datas = WeatherForecast.GenerateData() };
        }

        [HttpGet]
        [Route("get-direct-inherited-api-response")]
        public virtual InheritedCustomResponse GetDirectInheritedApiResponse()
        {
            return new InheritedCustomResponse()
            {
                Datas = WeatherForecast.GenerateData()
            };
        }

        [HttpGet]
        [Route("get-dynamic")]
        public virtual dynamic GetDynamic(int type)
        {
            return type switch
            {
                0 => WeatherForecast.GenerateData(),
                1 => new CustomResponse<WeatherForecast[]>() { Datas = WeatherForecast.GenerateData() },
                2 => new InheritedCustomResponse() { Datas = WeatherForecast.GenerateData() },
                _ => WeatherForecast.GenerateData(),
            };
        }

        [HttpGet]
        [Route("get-dynamic-task")]
        public async Task<dynamic> GetDynamicTaskAsync(int type)
        {
            await Task.Delay(1);

            return GetDynamic(type);
        }

        [HttpGet]
        [Route("get-dynamic-valuetask")]
        public async ValueTask<dynamic> GetDynamicValueTaskAsync(int type)
        {
            await Task.Delay(1);

            return GetDynamic(type);
        }

        [HttpGet]
        [AuthorizeMixed]
        [Route("get-authentication")]
        public WeatherForecast[] GetNeedAuthentication()
        {
            return WeatherForecast.GenerateData();
        }

        [HttpGet]
        [AuthorizeMixed("CanAccess")]
        [Route("get-authorize")]
        public WeatherForecast[] GetNeedAuthorize()
        {
            return WeatherForecast.GenerateData();
        }

        [HttpGet]
        [Route("get-nowrap")]
        [NoResponseWrap]
        public WeatherForecast[] GetNoWrap()
        {
            return WeatherForecast.GenerateData();
        }

        [HttpGet]
        [Route("get-task")]
        public async Task<WeatherForecast[]> GetTaskAsync()
        {
            await Task.Delay(1);
            return WeatherForecast.GenerateData();
        }

        [HttpGet]
        [Route("get-task-direct-api-response")]
        public virtual async Task<CustomResponse<WeatherForecast[]>> GetTaskDirectApiResponseAsync()
        {
            await Task.Delay(1);
            return new CustomResponse<WeatherForecast[]>() { Datas = WeatherForecast.GenerateData() };
        }

        [HttpGet]
        [Route("get-valuetask")]
        public async ValueTask<WeatherForecast[]> GetValueTaskAsync()
        {
            await Task.Delay(1);
            return WeatherForecast.GenerateData();
        }

        [HttpGet]
        [Route("get-valuetask-direct-api-response")]
        public virtual async ValueTask<CustomResponse<WeatherForecast[]>> GetValueTaskDirectApiResponseAsync()
        {
            await Task.Delay(1);
            return new CustomResponse<WeatherForecast[]>() { Datas = WeatherForecast.GenerateData() };
        }

        [HttpGet]
        [Route("get-exception-throw")]
        public WeatherForecast[] GetWithExceptionThrow()
        {
            throw new Exception("There has some exception");
        }

        [HttpGet]
        [Route("get-with-param")]
        public WeatherForecast[] GetWithParam(int? count)
        {
            return WeatherForecast.GenerateData(count ?? 5);
        }

        [HttpGet]
        [Route("get-with-param-required")]
        public WeatherForecast[] GetWithParamRequired([Required] int? count)
        {
            return WeatherForecast.GenerateData(count!.Value);
        }

        [HttpGet]
        [Route("get-with-param-required-limit")]
        public WeatherForecast[] GetWithParamRequiredAndLimit([Required][Range(1, 10)] int? count)
        {
            return WeatherForecast.GenerateData(count!.Value);
        }

        #endregion Public 方法
    }
}