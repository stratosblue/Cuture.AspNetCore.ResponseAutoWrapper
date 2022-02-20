# Cuture.AspNetCore.ResponseAutoWrapper
## 1. Intro
用于`asp.net core`的响应和异常自动包装器，使`Action`提供一致的响应内容格式

- 支持`Swagger`，能够正确展示包装后的类型结构；
- 支持自定义响应结构，自定义异常解析等；
- 基于`asp.net core`自身的特性实现，兼容性较好，性能影响较低（目前只做了初步的测试，在简单场景下，性能降低大概在`5%`左右）；
- 灵活的筛选方式，可以更准确的筛选出不需要包装的Action；

执行流程概览：
![执行流程概览](./execution_flow.png)

## 2. 注意项
- 目标框架`net6.0`、`net5.0`、`netcoreapp3.1`
- 包装功能由两个包装器实现：
    - 基于`ResultFilter`的`ActionResult`包装器：针对方法的返回值包装；
    - 基于`中间件`的包装器：针对异常、非200响应包装；
- 默认响应格式为
    ```json
    {
        "code": 200,  //状态码
        "message": "string", //消息
        "data": {}  //原本Action返回内容
    }
    ```
- 四个针对场景的包装器（都已经有默认实现，可以自行实现后注入DI容器，替换默认的功能）：
    - `IActionResultWrapper<TResponse>`: 针对`ActionResult`的包装器；
    - `IExceptionWrapper<TResponse>`: 针对`中间件中捕获到异常`的包装器；
    - `IInvalidModelStateWrapper<TResponse>`: `参数验证失败`的包装器；
    - `INotOKStatusCodeWrapper<TResponse>`: 中间件中`StatusCode`非`200`的响应包装器；
- 默认的`IActionResultWrapper`只会处理`ObjectResult`、`EmptyResult`、`StatusCodeResult`；
- 理论上能够与其它组件友好共存，如`Swagger`等，不过还没有做足够的测试；
- 自定义统一响应类型`TResponse`时，如果`启用了OpenAPI支持`，则其必须为泛型参数为`object`的泛型，例如：
    - 自定义统一响应类型如下：
        ```C#
        public class CustomResponse<TData>
        {
            public int Code { get; set; } = 200;
            public TData? Data { get; set; }
            public string? Info { get; set; }
        }
        ```
    - 则对应的创建器泛型参数必须为`CustomResponse<object>`，即：
        ```C#
        public class CustomResponseCreator : IResponseCreator<CustomResponse<object>>
        {
            public CustomResponse<object> Create(int code, string? message = null)
            {
                return new CustomResponse<object>(){Code = code,Info = message};
            }

            public CustomResponse<object> Create(int code, object? data, string? message = null)
            {
                return new CustomResponse<object>(){Code = code,Data = data,Info = message};
            }
        }
        ```
    - 则注册时其泛型参数必须为`object`，即：
        ```C#
        services.AddResponseAutoWrapper<CustomResponse<object>, CustomResponseCreator>();
        ```

### 可能存在问题的点
- `ResultFilter`中会频繁`未加锁`读取`ActionDescriptor.Properties`，如果存在不正确的写入，可能引发一些问题；
- 使用动态添加`ProducesResponseTypeAttribute`的方式实现的`OpenAPI`支持，可能存在不完善的地方；
- `asp.net core3.1`不支持`IAuthorizationMiddlewareResultHandler`，不支持选项`HandleAuthorizationResult`；
- `授权`和`认证`失败的包装需要手动指定对应组件的失败处理方法，否则可能无法包装；
- 参数验证失败的包装通过设置`ApiBehaviorOptions.InvalidModelStateResponseFactory`实现，可能有冲突；

## 3. 如何使用

### 3.1 安装`Nuget`包

```PowerShell
Install-Package Cuture.AspNetCore.ResponseAutoWrapper -IncludePrerelease
```

### 3.2 启用`ResultFilter`包装器

在`Startup.ConfigureServices`中启用

```C#
services.AddResponseAutoWrapper();
```

可以进行一些配置
```C#
services.AddResponseAutoWrapper(options =>
{
    //配置
    //options.ActionNoWrapPredicate Action的筛选委托，默认会过滤掉标记了NoResponseWrapAttribute的方法
    //options.DisableOpenAPISupport 禁用OpenAPI支持，Swagger将不会显示包装后的格式，也会解除响应类型必须为object泛型的限制
    //options.HandleAuthorizationResult 处理授权结果（可能无效，需要自行测试）
    //options.HandleInvalidModelState 处理无效模型状态
    //options.UseWrapper<TWrapper>() 使用指定类型的包装器替代默认包装器
});
```

默认情况下不会包装使用`[NoResponseWrapAttribute]`标记的方法；

### 3.3 启用中间件包装器

在`Startup.Configure`中配置

```C#
app.UseResponseAutoWrapper();
```

也可以进行一些配置
```C#
app.UseResponseAutoWrapper(options =>
{
    //配置
    //options.CatchExceptions 是否捕获异常
    //options.ThrowCaughtExceptions 捕获到异常处理结束后，是否再将异常抛出
    //options.DefaultOutputFormatterSelector 默认输出格式化器选择委托，选择在请求中无 Accept 时，用于格式化响应的 IOutputFormatter
});
```

-------

## 4. 定制化

### 4.1 自定义消息内容
 - 方法一：Action方法直接返回`TResponse`时，不会对其进行包装，默认`TResponse`为`ApiResponse<object>`，使用默认配置时，方法直接返回`ApiResponse`及其子类即可
    ```C#
    [HttpGet]
    [Route("cm")]
    public ApiResponse GetWithCustomMessage()
    {
        return EmptyApiResponse.Create("自定义消息");
    }
    ```

    返回结果为
    ```json
    {
    "data": null,
    "code": 200,
    "message": "自定义消息"
    }
    ```

 - 方法二：通过`Microsoft.AspNetCore.Http`命名空间下的`HttpContext`的`响应描述拓展方法`进行描述
    ```C#
    [HttpGet]
    public WeatherForecast[] Get()
    {
        HttpContext.DescribeResponse(10086, "Hello world!");
        return null;
    }
    ```

    返回结果为
    ```json
    {
    "data": null,
    "code": 10086,
    "message": "Hello world!"
    }
    ```

### 4.2 自定义统一响应类型`TResponse`
默认的`ApiResponse`不能满足需求时，可自行实现并替换`TResponse`

#### 方式一：使用`继承接口`实现自定义`TResponse`

 - 定义类型，并按需继承`ISetResponseCodeFeature`, `ISetResponseDataFeature`, `ISetResponseMessageFeature`

    ```C#
    public class CustomResponse<TData> : ISetResponseCodeFeature, ISetResponseDataFeature, ISetResponseMessageFeature
    {
        public int ResultCode { get; set; } = 200;
        public TData Result { get; set; }
        public string Msg { get; set; }
        public void SetCode(int code) => ResultCode = code;
        public void SetData(object? data) => Result = (TData)data;
        public void SetMessage(string? message) => Msg = message;
    }
    ```
 - 在配置时，指定使用的`TResponse`
    ```C#
    services.AddResponseAutoWrapper<CustomResponse<object>>();
    ```
    此时，统一响应内容变更为
    ```json
    {
    "resultCode": 0,
    "result": {},
    "msg": "string"
    }
    ```

------

#### 方式二：使用自定义`IResponseCreator`实现自定义`TResponse`
 - 自定义统一响应类型
    ```C#
    public class CustomResponse<TData>
    {
        public int StatusCode { get; set; } = 200;
        public TData Datas { get; set; }
        public string Info { get; set; }
    }
    ```
 - 实现`IResponseCreator<TResponse>`
    ```C#
    public class CustomResponseCreator : IResponseCreator<CustomResponse<object>>
    {
        public CustomResponse<object> Create(int code, string? message = null)
        {
            return new CustomResponse<object>()
            {
                StatusCode = code,
                Info = message,
            };
        }

        public CustomResponse<object> Create(int code, object? data, string? message = null)
        {
            return new CustomResponse<object>()
            {
                StatusCode = code,
                Datas = data,
                Info = message,
            };
        }
    }
    ```
 - 在配置时，指定使用的`TResponse`和`IResponseCreator`
    ```C#
    services.AddResponseAutoWrapper<CustomResponse<object>, CustomResponseCreator>();
    ```
    此时，统一响应内容变更为
    ```json
    {
    "statusCode": 0,
    "datas": {},
    "info": "string"
    }
    ```

## Note!!!
仅当`禁用OpenAPI支持时`，`TResponse`才能不是一个泛型参数为`object`的泛型；
可参考`test/ResponseAutoWrapper.TestHost`项目；

### 4.3 其它自定义

使用自行实现的接口注入DI容器替换掉默认实现即可完成一些其它的自定义

- `IActionResultWrapper<TResponse>`: ActionResult包装器；
- `IExceptionWrapper<TResponse>`: 捕获异常时的响应包装器；
- `IInvalidModelStateWrapper<TResponse>`: 模型验证失败时的响应包装器；
- `INotOKStatusCodeWrapper<TResponse>`: 非200状态码时的响应包装器；
- `IExceptionMessageProvider`: 从异常解析返回消息的消息提供器；
- `IWrapTypeCreator`: 确认Action返回对象类型是否需要包装，以及创建OpenAPI展示的泛型类；

## 5. 性能测试结果

- 系统：`Ubuntu20.04 on WSL2` host by Windows10-21H1
- CPU：`I7-8700`
- 平台：`asp.net core 5.0`
- 测试软件：`wrk`
- 测试软件参数：`-t 3 -c 100 -d 30s`
- 测试Action为：
    ```C#
    [HttpGet]
    public IEnumerable<WeatherForecast> Get(int count = 5)
    {
        return WeatherForecast.GenerateData(count);
    }
    ```
- 测试均为使用`localhost`，以尽量减少网络的影响；
- 对比对象分别为：
    - Origin：原生，没有进行包装
    - Cuture.AspNetCore.ResponseAutoWrapper：此自动包装库
    - [AutoWrapper.Core](https://github.com/proudmonkey/AutoWrapper)：另一个同类型的自动包装库
- 结果为多次运行取峰值；
- 测试环境不专业，会有一定的误差，数值仅供参考；

### 数据（单位 `Requests/sec`）
|`count`      |      Origin              |Cuture.AspNetCore.ResponseAutoWrapper|   AutoWrapper.Core      |
|   ----      |   ----                   |                ----                 |         ----            |
| 1           |123267.40                 |111868.34                            |91202.04                 |
| 10          |108264.80                 |103125.32                            |67001.92                 |
| 50          |76310.72                  |73451.83                             |32275.47                 |
