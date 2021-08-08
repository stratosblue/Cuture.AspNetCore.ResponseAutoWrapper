using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Http;

namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal
{
    /// <summary>
    /// 默认的 <inheritdoc cref="IMessageProvider"/>
    /// </summary>
    internal class DefaultMessageProvider : IMessageProvider
    {
        #region Private 字段

        private readonly Dictionary<int, string> _codeMap;

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="DefaultMessageProvider"/>
        public DefaultMessageProvider()
        {
            // [\r\n\S\s]+? int [a-z]+[0-9]+(.+?) = (\d+);
            // =>
            // _codeMap.TryAdd($2, "$1");\n

            _codeMap = new Dictionary<int, string>();

            _codeMap.TryAdd(100, "Continue");
            _codeMap.TryAdd(101, "SwitchingProtocols");
            _codeMap.TryAdd(102, "Processing");
            _codeMap.TryAdd(200, "OK");
            _codeMap.TryAdd(201, "Created");
            _codeMap.TryAdd(202, "Accepted");
            _codeMap.TryAdd(203, "NonAuthoritative");
            _codeMap.TryAdd(204, "NoContent");
            _codeMap.TryAdd(205, "ResetContent");
            _codeMap.TryAdd(206, "PartialContent");
            _codeMap.TryAdd(207, "MultiStatus");
            _codeMap.TryAdd(208, "AlreadyReported");
            _codeMap.TryAdd(226, "IMUsed");
            _codeMap.TryAdd(300, "MultipleChoices");
            _codeMap.TryAdd(301, "MovedPermanently");
            _codeMap.TryAdd(302, "Found");
            _codeMap.TryAdd(303, "SeeOther");
            _codeMap.TryAdd(304, "NotModified");
            _codeMap.TryAdd(305, "UseProxy");
            _codeMap.TryAdd(306, "SwitchProxy");
            _codeMap.TryAdd(307, "TemporaryRedirect");
            _codeMap.TryAdd(308, "PermanentRedirect");
            _codeMap.TryAdd(400, "BadRequest");
            _codeMap.TryAdd(401, "Unauthorized");
            _codeMap.TryAdd(402, "PaymentRequired");
            _codeMap.TryAdd(403, "Forbidden");
            _codeMap.TryAdd(404, "NotFound");
            _codeMap.TryAdd(405, "MethodNotAllowed");
            _codeMap.TryAdd(406, "NotAcceptable");
            _codeMap.TryAdd(407, "ProxyAuthenticationRequired");
            _codeMap.TryAdd(408, "RequestTimeout");
            _codeMap.TryAdd(409, "Conflict");
            _codeMap.TryAdd(410, "Gone");
            _codeMap.TryAdd(411, "LengthRequired");
            _codeMap.TryAdd(412, "PreconditionFailed");
            _codeMap.TryAdd(413, "RequestEntityTooLarge");
            _codeMap.TryAdd(413, "PayloadTooLarge");
            _codeMap.TryAdd(414, "RequestUriTooLong");
            _codeMap.TryAdd(414, "UriTooLong");
            _codeMap.TryAdd(415, "UnsupportedMediaType");
            _codeMap.TryAdd(416, "RequestedRangeNotSatisfiable");
            _codeMap.TryAdd(416, "RangeNotSatisfiable");
            _codeMap.TryAdd(417, "ExpectationFailed");
            _codeMap.TryAdd(418, "ImATeapot");
            _codeMap.TryAdd(419, "AuthenticationTimeout");
            _codeMap.TryAdd(421, "MisdirectedRequest");
            _codeMap.TryAdd(422, "UnprocessableEntity");
            _codeMap.TryAdd(423, "Locked");
            _codeMap.TryAdd(424, "FailedDependency");
            _codeMap.TryAdd(426, "UpgradeRequired");
            _codeMap.TryAdd(428, "PreconditionRequired");
            _codeMap.TryAdd(429, "TooManyRequests");
            _codeMap.TryAdd(431, "RequestHeaderFieldsTooLarge");
            _codeMap.TryAdd(451, "UnavailableForLegalReasons");
            _codeMap.TryAdd(500, "InternalServerError");
            _codeMap.TryAdd(501, "NotImplemented");
            _codeMap.TryAdd(502, "BadGateway");
            _codeMap.TryAdd(503, "ServiceUnavailable");
            _codeMap.TryAdd(504, "GatewayTimeout");
            _codeMap.TryAdd(505, "HttpVersionNotsupported");
            _codeMap.TryAdd(506, "VariantAlsoNegotiates");
            _codeMap.TryAdd(507, "InsufficientStorage");
            _codeMap.TryAdd(508, "LoopDetected");
            _codeMap.TryAdd(510, "NotExtended");
            _codeMap.TryAdd(511, "NetworkAuthenticationRequired");
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetMessage(int code) => _codeMap.TryGetValue(code, out var value) ? value : "Error";

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetMessage(int code, HttpContext? context) => GetMessage(code);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetMessage(int code, HttpContext? context, Exception? exception) => GetMessage(code);

        #endregion Public 方法
    }
}