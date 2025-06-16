namespace StreamChat.Clients
{
    /// <summary>
    /// Factory class for clients that can be used to interract with the Chat API.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/?language=csharp</remarks>
    public interface IStreamClientFactory
    {
        /// <summary>
        /// Returns an <see cref="IAppClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/app_setting_overview/?language=csharp</remarks>
        IAppClient GetAppClient();

        /// <summary>
        /// Returns an <see cref="IBlocklistClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/block_lists/?language=csharp</remarks>
        IBlocklistClient GetBlocklistClient();

        /// <summary>
        /// Returns an <see cref="IChannelClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/creating_channels/?language=csharp</remarks>
        IChannelClient GetChannelClient();

        /// <summary>
        /// Returns an <see cref="IChannelTypeClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_features/?language=csharp</remarks>
        IChannelTypeClient GetChannelTypeClient();

        /// <summary>
        /// Returns an <see cref="ICommandClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/custom_commands_webhook/?language=csharp</remarks>
        ICommandClient GetCommandClient();

        /// <summary>
        /// Returns an <see cref="IDeviceClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/push_devices/?language=csharp</remarks>
        IDeviceClient GetDeviceClient();

        /// <summary>
        /// Returns an <see cref="IEventClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/custom_events/?language=csharp</remarks>
        IEventClient GetEventClient();

        /// <summary>
        /// Returns an <see cref="IFlagClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        IFlagClient GetFlagClient();

        /// <summary>
        /// Returns an <see cref="IImportClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/import/?language=csharp</remarks>
        IImportClient GetImportClient();

        /// <summary>
        /// Returns an <see cref="IMessageClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        IMessageClient GetMessageClient();

        /// <summary>
        /// Returns an <see cref="IPermissionClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/chat_permission_policies/?language=csharp</remarks>
        IPermissionClient GetPermissionClient();

        /// <summary>
        /// Returns an <see cref="IReactionClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_reaction/?language=csharp</remarks>
        IReactionClient GetReactionClient();

        /// <summary>
        /// Returns an <see cref="ITaskClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/rest/#tasks-gettask</remarks>
        ITaskClient GetTaskClient();

        /// <summary>
        /// Returns an <see cref="IUserClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/tokens_and_authentication/?language=csharp</remarks>
        IUserClient GetUserClient();

        /// <summary>
        /// Gets a client that can be used to access moderation endpoints.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        IModerationClient GetModerationClient();

        /// <summary>
        /// Returns an <see cref="IThreadClient"/> instance. The returned client can be used as a singleton in your application.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/threads/?language=csharp#filtering-and-sorting-threads</remarks>
        IThreadClient GetThreadClient();
    }
}
