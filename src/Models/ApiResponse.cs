using System;

namespace StreamChat.Models
{
    /// <summary>
    /// ApiResponse contains fields that every Stream response includes.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Duration of the request in human-readable format.
        /// </summary>
        public string Duration { get; set; }

        private RateLimitsInfo _rateLimits;

        internal void SetRateLimit(RateLimitsInfo rateLimits) => _rateLimits = rateLimits;

        /// <summary>
        /// Provides rate limit information for the request.
        /// Throws <see cref="MissingRateLimitException"/> if the rate limit information wasn't available
        /// in the response. Use <see cref="TryGetRateLimit"/> for the exceptionless version.
        /// </summary>
        /// <exception cref="MissingRateLimitException">If the ratelimit information is unavailable.</exception>
        public RateLimitsInfo GetRateLimit()
        {
            if (!TryGetRateLimit(out var rateLimit))
                throw new MissingRateLimitException("Rate limit information was not available in the response.");

            return rateLimit;
        }

        /// <summary>
        /// Provides rate limit information for the request.
        /// </summary>
        public bool TryGetRateLimit(out RateLimitsInfo rateLimits)
        {
            rateLimits = _rateLimits;

            return rateLimits != null;
        }
    }

    public class MissingRateLimitException : Exception
    {
        internal MissingRateLimitException(string message) : base(message)
        {
        }
    }
}