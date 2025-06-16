using System;
using StreamChat.Clients;

namespace StreamChatTests
{
    public static class TestClientFactory
    {
        private static IStreamClientFactory _clientFactory = new StreamClientFactory(
                Environment.GetEnvironmentVariable("STREAM_KEY"),
                Environment.GetEnvironmentVariable("STREAM_SECRET"),
                opts => opts.Timeout = TimeSpan.FromSeconds(10));

        public static IAppClient GetAppClient() => _clientFactory.GetAppClient();
        public static IBlocklistClient GetBlocklistClient() => _clientFactory.GetBlocklistClient();
        public static IChannelClient GetChannelClient() => _clientFactory.GetChannelClient();
        public static IChannelTypeClient GetChannelTypeClient() => _clientFactory.GetChannelTypeClient();
        public static ICommandClient GetCommandClient() => _clientFactory.GetCommandClient();
        public static IDeviceClient GetDeviceClient() => _clientFactory.GetDeviceClient();
        public static IEventClient GetEventClient() => _clientFactory.GetEventClient();
        public static IFlagClient GetFlagClient() => _clientFactory.GetFlagClient();
        public static IImportClient GetImportClient() => _clientFactory.GetImportClient();
        public static IMessageClient GetMessageClient() => _clientFactory.GetMessageClient();
        public static IUserClient GetUserClient() => _clientFactory.GetUserClient();
        public static IPermissionClient GetPermissionClient() => _clientFactory.GetPermissionClient();
        public static IReactionClient GetReactionClient() => _clientFactory.GetReactionClient();
        public static ITaskClient GetTaskClient() => _clientFactory.GetTaskClient();
        public static IModerationClient GetModerationClient() => _clientFactory.GetModerationClient();
        public static IThreadClient GetThreadClient() => _clientFactory.GetThreadClient();
    }
}
