#if NETCOREAPP3_1

using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Http
{
    internal static class HttpResponseExtensions
    {
        #region Public 字段

        public const string JsonContentTypeWithCharset = "application/json; charset=utf-8";

        #endregion Public 字段

        #region Internal 字段

        internal static readonly JsonSerializerOptions s_defaultSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        #endregion Internal 字段

        #region Public 方法

        public static Task WriteAsJsonAsync<TValue>(this HttpResponse response,
                                                    TValue value,
                                                    CancellationToken cancellationToken = default)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var options = ResolveSerializerOptions(response.HttpContext);

            response.ContentType = JsonContentTypeWithCharset;
            return JsonSerializer.SerializeAsync<TValue>(response.Body, value, options, cancellationToken);
        }

        #endregion Public 方法

        #region Private 方法

        private static JsonSerializerOptions ResolveSerializerOptions(HttpContext httpContext)
        {
            var options = httpContext.RequestServices?.GetService(typeof(IOptions<JsonOptions>)) as IOptions<JsonOptions>;
            // Attempt to resolve options from DI then fallback to default options
            return options?.Value?.JsonSerializerOptions ?? s_defaultSerializerOptions;
        }

        #endregion Private 方法
    }
}

#endif