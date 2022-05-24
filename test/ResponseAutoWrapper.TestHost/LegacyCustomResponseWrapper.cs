using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.Extensions.Options;

namespace ResponseAutoWrapper.TestHost;

public class NotGenericLegacyCustomResponseWrapper : LegacyCompatibleResponseWrapper<LegacyCustomResponse>
{
    #region Public 构造函数

    public NotGenericLegacyCustomResponseWrapper(IWrapTypeCreator<int, string> wrapTypeCreator, IOptions<ResponseAutoWrapperOptions> optionsAccessor, IOptions<LegacyCompatibleResponseWrapperOptions> wrapperOptionsAccessor)
        : base(wrapTypeCreator, optionsAccessor, wrapperOptionsAccessor)
    {
    }

    #endregion Public 构造函数

    #region Protected 方法

    protected override LegacyCustomResponse? CreateResponse(int code) => new() { StatusCode = code };

    protected override LegacyCustomResponse? CreateResponse(int code, string? message) => new() { StatusCode = code, Info = message };

    protected override LegacyCustomResponse? CreateResponse(int code, string? message, object? data) => new() { StatusCode = code, Info = message, Datas = data };

    #endregion Protected 方法
}

public class LegacyCustomResponseWrapper : LegacyCompatibleResponseWrapper<LegacyCustomResponse<object>>
{
    #region Public 构造函数

    public LegacyCustomResponseWrapper(IWrapTypeCreator<int, string> wrapTypeCreator, IOptions<ResponseAutoWrapperOptions> optionsAccessor, IOptions<LegacyCompatibleResponseWrapperOptions> wrapperOptionsAccessor)
        : base(wrapTypeCreator, optionsAccessor, wrapperOptionsAccessor)
    {
    }

    #endregion Public 构造函数

    #region Protected 方法

    protected override LegacyCustomResponse<object>? CreateResponse(int code) => new() { StatusCode = code };

    protected override LegacyCustomResponse<object>? CreateResponse(int code, string? message) => new() { StatusCode = code, Info = message };

    protected override LegacyCustomResponse<object>? CreateResponse(int code, string? message, object? data) => new() { StatusCode = code, Info = message, Datas = data };

    #endregion Protected 方法
}
