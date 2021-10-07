using StreamChat;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="Client"/> to the service collection.
        /// <param name="apiKey">The Stream API key.</param>
        /// <param name="apiSecret">Your Stream API secret.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddStreamClient(this IServiceCollection services, string apiKey, string apiSecret)
            => services.AddSingleton<IClient>(new Client(apiKey, apiSecret));

        /// <summary>
        /// Adds the <see cref="Client"/> to the service collection.
        /// <param name="apiKey">The Stream API key.</param>
        /// <param name="apiSecret">Your Stream API secret.</param>
        /// <param name="opts">A <see cref="ClientOptions"/> instance to pass into the client.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddStreamClient(this IServiceCollection services, string apiKey, string apiSecret, ClientOptions opts)
            => services.AddSingleton<IClient>(new Client(apiKey, apiSecret, opts));
    }
}