using System.Collections.Generic;
using System.Net;
using StreamChat.Models;
using StreamChat.Rest;
using StreamChat.Utils;

namespace StreamChat.Exceptions
{
    /// <summary>
    /// Thrown when the backend returns an unexpected HTTP status code.
    /// </summary>
    public class StreamChatException : StreamBaseException
    {
        private StreamChatException(ExceptionResponse resp) : base(resp.Message)
        {
            ExceptionFields = resp.ExceptionFields;
            MoreInfo = resp.MoreInfo;
            ErrorCode = resp.Code;
            HttpStatusCode = resp.HttpStatusCode;
            RateLimit = resp.RateLimit;
        }

        /// <summary>Error code from the backend.</summary>
        public int ErrorCode { get; }

        /// <summary>HTTP code returned from the backend.</summary>
        public HttpStatusCode HttpStatusCode { get; }

        /// <summary>Rate limit information returned from the backend.</summary>
        public RateLimitsInfo RateLimit { get; }

        /// <summary>Exception fields from the backend.</summary>
        public Dictionary<string, string> ExceptionFields { get; }

        /// <summary>A link that provides more information about the nature of the error.</summary>
        public string MoreInfo { get; }

        internal static StreamChatException FromResponse(RestResponse response)
        {
            var resp = new ExceptionResponse { HttpStatusCode = response.StatusCode };

            if (!string.IsNullOrWhiteSpace(response.Content))
            {
                resp = StreamJsonConverter.DeserializeObject<ExceptionResponse>(response.Content);
                resp.HttpStatusCode = response.StatusCode;
            }

            if (response.TryGetRateLimit(out var rateLimit))
            {
                resp.RateLimit = rateLimit;
            }

            return new StreamChatException(resp);
        }
    }

    internal class ExceptionResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string> ExceptionFields { get; set; }
        public string MoreInfo { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public RateLimitsInfo RateLimit { get; set; }
    }
}
