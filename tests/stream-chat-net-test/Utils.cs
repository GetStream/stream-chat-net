using System;

namespace StreamChatTests
{
    public class Credentials
    {
        public static Credentials Instance = new Credentials();

        public StreamChat.IClient Client
        {
            get
            {
                return _client;
            }
        }


        private readonly StreamChat.Client _client;

        internal Credentials()
        {
            var apiKey = Environment.GetEnvironmentVariable("STREAM_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("STREAM_API_SECRET");
            _client = new StreamChat.Client(apiKey, apiSecret,
                new StreamChat.ClientOptions()
                {
                    Timeout = 10000
                });
        }
    }
}
