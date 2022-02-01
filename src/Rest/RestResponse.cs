using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Rest
{
    internal class RestResponse
    {
        private static readonly DateTimeOffset _epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        internal HttpResponseHeaders Headers { get; private set; }

        internal HttpStatusCode StatusCode { get; private set; }

        internal string Content { get; private set; }

        internal static async Task<RestResponse> FromResponseMessageAsync(HttpResponseMessage message)
        {
            var response = new RestResponse
            {
                StatusCode = message.StatusCode,
                Headers = message.Headers,
            };

            using (message)
            {
                response.Content = await message.Content.ReadAsStringAsync();
            }

            return response;
        }

        internal bool TryGetRateLimit(out RateLimitsInfo rateLimit)
        {
            if (Headers.TryGetValues("X-Ratelimit-Limit", out var limit)
                && Headers.TryGetValues("X-Ratelimit-Remaining", out var remaining)
                && Headers.TryGetValues("X-Ratelimit-Reset", out var reset))
            {
                rateLimit = new RateLimitsInfo
                {
                    Limit = int.Parse(limit.First()),
                    Remaining = int.Parse(remaining.First()),
                    Reset = _epoch.AddSeconds(int.Parse(reset.First())),
                };

                return true;
            }

            rateLimit = null;

            return false;
        }
    }
}
