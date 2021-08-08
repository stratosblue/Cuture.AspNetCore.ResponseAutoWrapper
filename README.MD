# Cuture.AspNetCore.ResponseAutoWrapper
## 1. Intro
用于`asp.net core`的响应和异常自动包装器，使`Action`提供一致的响应内容格式

## 2. 注意项
- 目标框架`net5.0`、`netcoreapp3.1`
- 包含两个包装器：
    - 基于`ResultFilter`的`ActionResult`包装器：针对方法的返回值包装；
    - 基于`中间件`的包装器：针对异常、非200响应包装；
- 默认响应格式为
    ```json
    {
        "code": 200,  //状态码
        "message": "string", //消息
        "data": {}  //原本Action返回内容
    }
- 理论上能够与其它组件友好共存，如`Swagger`等，不过还没有做足够的测试；

### 可能存在问题的点
- `ResultFilter`中会频繁`未加锁`读取`ActionDescriptor.Properties`，如果存在不正确的写入，可能引发一些问题；
- `ResultFilter`中只处理了`ObjectResult`和`EmptyResult`，暂时还不明确其他情况；
- 使用动态添加`ProducesResponseTypeAttribute`的方式实现的`OpenAPI`支持，可能存在不完善的地方；
- `asp.net core3.1`不支持`IAuthorizationMiddlewareResultHandler`，不支持选项`HandleAuthorizationResult`；
- `授权`和`认证`失败的包装需要手动指定对应组件的失败处理方法，否则可能无法包装；

    ```
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
    //options.MiddlewareStatusCodePredicate 状态码的筛选委托，默认会过滤掉3**的状态码
    //options.CatchExceptions 是否捕获异常
    //options.ThrowCaughtExceptions 捕获到异常处理结束后，是否再将异常抛出
});
```

-------

## 4. 定制化

### 4.1 自定义消息内容
方法直接返回`TResponse`时，不会对其进行包装，默认`TResponse`为`ApiResponse<object>`，使用默认配置时，方法直接返回`ApiResponse`及其子类即可
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

### 4.2 自定义统一响应类型`TResponse`
默认的`ApiResponse`不能满足需求时，可自行实现并替换`TResponse`

#### 方式一：使用`继承接口`实现自定义`TResponse`

 - 定义类型，并按需继承`ISetResponseCode`, `ISetResponseData`, `ISetResponseException`, `ISetResponseMessage`

    ```C#
    public class CustomResponse<TData> : ISetResponseCode, ISetResponseData, ISetResponseException, ISetResponseMessage
    {
        public int ResultCode { get; set; } = 200;
        public TData Result { get; set; }
        public string Msg { get; set; }
        public string Error { get; set; }
        public void SetCode(int code) => ResultCode = code;
        public void SetData(object? data) => Result = (TData)data;
        public void SetException(Exception? exception)
        {
            if (exception is not null)
            {
                ResultCode = 500;
                Msg = exception.Message;
                Error = exception.StackTrace;
            }
        }
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
  "msg": "string",
  "error": "string"
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
        public string ErrorInfo { get; set; }
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

        public CustomResponse<object> Create(int code, Exception exception, string? message = null)
        {
            return new CustomResponse<object>()
            {
                StatusCode = code,
                Info = message,
                ErrorInfo = exception?.StackTrace,
            };
        }

        public object CreateObject(int code, string? message = null, Exception? exception = null)
        {
            return Create(code, exception, message);
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
  "info": "string",
  "errorInfo": "string"
}
```

## Note!!!
仅当`禁用OpenAPI支持时`，`TResponse`才能不是一个泛型参数为`object`的泛型；
可参考`test/ResponseAutoWrapper.TestHost`项目；

### 4.3 其它自定义

使用自行实现的接口注入DI容器替换掉默认实现即可完成一些其它的自定义

- `IInvalidModelStateResponseFormatter<TResponse>`: 模型验证失败时的响应处理器；
- `IMessageProvider`: 在中间件中处理非成功状态的http请求时，用以提供返回消息；
- `IResponseDirectWriter`: 用于中间件中写入响应，默认为写入json；
- `IWrapTypeCreator`: 确认Action返回对象类型是否需要包装，以及创建OpenAPI展示的泛型类；
