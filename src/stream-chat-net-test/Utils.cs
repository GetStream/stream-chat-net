using System;
using System.Net.Http;

namespace StreamChatTests
{
    public static class TestClientFactory
    {
        private static string _apiKey = Environment.GetEnvironmentVariable("STREAM_API_KEY");
        private static string _apiSecret = Environment.GetEnvironmentVariable("STREAM_API_SECRET");
        private static StreamChat.Client _defaultClient = new StreamChat.Client(_apiKey, _apiSecret,
                new StreamChat.ClientOptions
                {
                    Timeout = 10000
                });

        public static StreamChat.IClient GetClient() => _defaultClient;

        public static StreamChat.IClient GetClient(HttpClient httpClient)
            => new StreamChat.Client(
                _apiKey,
                _apiSecret,
                httpClient,
                new StreamChat.ClientOptions{ Timeout = 10000 }
            );
        
    }
}
