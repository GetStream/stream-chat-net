using System;
using System.Net.Http;

namespace StreamChat.Models
{
    /// <summary>
    /// Provides custom options for the underlying HTTP client.
    /// </summary>
    public class ClientOptions
    {
        private static readonly HttpClient DefaultHttpClient = new HttpClient();

        /// <summary>The <see cref="System.Net.Http.HttpClient"/> instance used for underlying HTTP calls.</summary>
        public HttpClient HttpClient { get; set; } = DefaultHttpClient;

        /// <summary>Timeout of HTTP requests.</summary>
        /// <remarks>Default is 10 seconds.</remarks>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>The base url for the Stream Chat API.</summary>
        /// <remarks>Default is https://chat.stream-io-api.com.</remarks>
        public Uri BaseUrl { get; set; } = new Uri("https://chat.stream-io-api.com");

#if NETCOREAPP2_1_OR_GREATER
        // SocketsHttpHandler is only available in .NET Core 2.1+
        static ClientOptions()
        {
            // AWS load balancer disconnects in 60 seconds, so let's make it 59
            var socketsHandler = new SocketsHttpHandler();
            socketsHandler.PooledConnectionLifetime = TimeSpan.FromSeconds(59);
            DefaultHttpClient = new HttpClient(socketsHandler);
        }
#endif
        internal void OverrideWithEnvVars()
        {
            var timeoutEnvVar = Environment.GetEnvironmentVariable("STREAM_CHAT_TIMEOUT");
            var urlEnvVar = Environment.GetEnvironmentVariable("STREAM_CHAT_URL");

            if (!string.IsNullOrWhiteSpace(timeoutEnvVar))
                Timeout = TimeSpan.FromSeconds(int.Parse(timeoutEnvVar));

            if (!string.IsNullOrWhiteSpace(urlEnvVar))
                BaseUrl = new Uri(urlEnvVar, UriKind.Absolute);
        }
    }
}
