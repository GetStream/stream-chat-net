using System;
using System.Reflection;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    /// <summary>
    /// Factory class for clients that can be used to interract with the Chat API.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/?language=csharp</remarks>
    public class StreamClientFactory : IStreamClientFactory
    {
        private readonly IAppClient _appClient;
        private readonly BlocklistClient _blocklistClient;
        private readonly IChannelClient _channelClient;
        private readonly IChannelTypeClient _channelTypeClient;
        private readonly CommandClient _commandClient;
        private readonly IDeviceClient _deviceClient;
        private readonly IEventClient _eventClient;
        private readonly ImportClient _importClient;
        private readonly IFlagClient _flagClient;
        private readonly IMessageClient _messageClient;
        private readonly IUserClient _userClient;
        private readonly IPermissionClient _permissionClient;
        private readonly IReactionClient _reactionClient;
        private readonly ITaskClient _taskClient;
        private readonly IModerationClient _moderationClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamClientFactory"/> class.
        /// This constructor uses STREAM_KEY and STREAM_SECRET environment
        /// variables to initialize the client.
        /// If they don't exist, <see cref="ArgumentNullException"/> will be thrown.
        /// </summary>
        /// <exception cref="ArgumentNullException">If API key or API secret is null.</exception>
        public StreamClientFactory() : this(null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamClientFactory"/> class.
        /// </summary>
        /// <param name="apiKey">The API key of a Stream Chat application.</param>
        /// <param name="apiSecret">The API secret of a Stream Chat application.</param>
        /// <exception cref="ArgumentNullException">If API key or API secret is null.</exception>
        public StreamClientFactory(string apiKey, string apiSecret) : this(apiKey, apiSecret, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamClientFactory"/> class.
        /// </summary>
        /// <param name="apiKey">The API key of a Stream Chat application.</param>
        /// <param name="apiSecret">The API secret of a Stream Chat application.</param>
        /// <param name="clientOptionsConfigurer">An action to configure <see cref="ClientOptions"/>.</param>
        /// <exception cref="ArgumentNullException">If API key or API secret is null.</exception>
        public StreamClientFactory(string apiKey, string apiSecret, Action<ClientOptions> clientOptionsConfigurer)
        {
            apiKey = apiKey ?? Environment.GetEnvironmentVariable("STREAM_KEY");
            apiSecret = apiSecret ?? Environment.GetEnvironmentVariable("STREAM_SECRET");

            var errMsg = "Provide an {0} or have it available in {1} environment variable.";
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentNullException(nameof(apiKey), string.Format(errMsg, nameof(apiKey), "STREAM_KEY"));
            if (string.IsNullOrWhiteSpace(apiSecret))
                throw new ArgumentNullException(nameof(apiSecret), string.Format(errMsg, nameof(apiSecret), "STREAM_SECRET"));

            var opts = new ClientOptions();
            opts.OverrideWithEnvVars();
            clientOptionsConfigurer?.Invoke(opts);
            opts.EnsureValid();

            var jwtGeneratorClient = new JwtGeneratorClient();
            var generatedJwt = jwtGeneratorClient.GenerateServerSideJwt(apiSecret);
            var assemblyVersion = typeof(StreamClientFactory).GetTypeInfo().Assembly.GetName().Version;
            var sdkVersion = assemblyVersion.ToString(3);
            var restClient = new RestClient(opts, generatedJwt, apiKey, sdkVersion);

            _appClient = new AppClient(restClient, apiSecret);
            _blocklistClient = new BlocklistClient(restClient);
            _channelClient = new ChannelClient(restClient);
            _channelTypeClient = new ChannelTypeClient(restClient);
            _commandClient = new CommandClient(restClient);
            _deviceClient = new DeviceClient(restClient);
            _eventClient = new EventClient(restClient);
            _importClient = new ImportClient(restClient);
            _flagClient = new FlagClient(restClient);
            _messageClient = new MessageClient(restClient);
            _permissionClient = new PermissionClient(restClient);
            _reactionClient = new ReactionClient(restClient);
            _taskClient = new TaskClient(restClient);
            _userClient = new UserClient(restClient, jwtGeneratorClient, apiSecret);
            _moderationClient = new ModerationClient(restClient);
        }

        public IAppClient GetAppClient() => _appClient;
        public IBlocklistClient GetBlocklistClient() => _blocklistClient;
        public IChannelClient GetChannelClient() => _channelClient;
        public IChannelTypeClient GetChannelTypeClient() => _channelTypeClient;
        public ICommandClient GetCommandClient() => _commandClient;
        public IDeviceClient GetDeviceClient() => _deviceClient;
        public IEventClient GetEventClient() => _eventClient;
        public IFlagClient GetFlagClient() => _flagClient;
        public IImportClient GetImportClient() => _importClient;
        public IMessageClient GetMessageClient() => _messageClient;
        public IPermissionClient GetPermissionClient() => _permissionClient;
        public IReactionClient GetReactionClient() => _reactionClient;
        public ITaskClient GetTaskClient() => _taskClient;
        public IUserClient GetUserClient() => _userClient;
        public IModerationClient GetModerationClient() => _moderationClient;
    }
}