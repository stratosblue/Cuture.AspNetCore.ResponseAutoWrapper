using Cuture.AspNetCore.ResponseAutoWrapper;

using Microsoft.Extensions.Options;

namespace ResponseAutoWrapper.TestHost;

public class NotGenericLegacyCustomResponseWrapper(IWrapTypeCreator<int, string> wrapTypeCreator, IOptions<ResponseAutoWrapperOptions> optionsAccessor, IOptions<LegacyCompatibleResponseWrapperOptions> wrapperOptionsAccessor)
    : LegacyCompatibleResponseWrapper<LegacyCustomResponse>(wrapTypeCreator, optionsAccessor, wrapperOptionsAccessor)
{

    #region Protected 方法

    protected override LegacyCustomResponse? CreateResponse(int code) => new() { StatusCode = code };

    protected override LegacyCustomResponse? CreateResponse(int code, string? message) => new() { StatusCode = code, Info = message };

    protected override LegacyCustomResponse? CreateResponse(int code, string? message, object? data) => new() { StatusCode = code, Info = message, Datas = data };

    #endregion Protected 方法
}

public class LegacyCustomResponseWrapper(IWrapTypeCreator<int, string> wrapTypeCreator, IOptions<ResponseAutoWrapperOptions> optionsAccessor, IOptions<LegacyCompatibleResponseWrapperOptions> wrapperOptionsAccessor)
    : LegacyCompatibleResponseWrapper<LegacyCustomResponse<object>>(wrapTypeCreator, optionsAccessor, wrapperOptionsAccessor)
{

    #region Protected 方法

    protected override LegacyCustomResponse<object>? CreateResponse(int code) => new() { StatusCode = code };

    protected override LegacyCustomResponse<object>? CreateResponse(int code, string? message) => new() { StatusCode = code, Info = message };

    protected override LegacyCustomResponse<object>? CreateResponse(int code, string? message, object? data) => new() { StatusCode = code, Info = message, Datas = data };

    #endregion Protected 方法
}
