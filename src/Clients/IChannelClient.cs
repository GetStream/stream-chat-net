using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to retrieve, create and alter channels of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/creating_channels/?language=csharp</remarks>
    public interface IChannelClient
    {
        /// <summary>
        /// Adds members to a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_members/?language=csharp</remarks>
        Task<UpdateChannelResponse> AddMembersAsync(string channelType, string channelId, params string[] userIds);

        /// <summary>
        /// Adds members to a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_members/?language=csharp</remarks>
        Task<UpdateChannelResponse> AddMembersAsync(string channelType, string channelId, IEnumerable<string> userIds,
            MessageRequest msg, AddMemberOptions options);

        /// <summary>
        /// Makes a member a moderator in a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        Task<ApiResponse> AddModeratorsAsync(string channelType, string channelId, IEnumerable<string> userIds);

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/chat_permission_policies/?language=csharp</remarks>
        Task<UpdateChannelResponse> AssignRolesAsync(string channelType, string channelId,
            AssignRoleRequest roleRequest);

        /// <summary>
        /// Deletes a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_delete/?language=csharp</remarks>
        Task<ApiResponse> DeleteAsync(string channelType, string channelId);

        /// <summary>
        /// <para>Deletes multiple channels.</para>
        /// This is an asynchronous operation and the returned value is a task Id.
        /// You can use <see cref="ITaskClient.GetTaskStatusAsync"/> to check the status of the task.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_delete/?language=csharp</remarks>
        Task<AsyncOperationResponse> DeleteChannelsAsync(IEnumerable<string> cids, bool hardDelete = false);

        /// <summary>
        /// <para>Updates channels in batch based on the provided options.</para>
        /// This is an asynchronous operation and the returned value is a task Id.
        /// You can use <see cref="ITaskClient.GetTaskStatusAsync"/> to check the status of the task.
        /// </summary>
        Task<AsyncOperationResponse> UpdateChannelsBatchAsync(ChannelsBatchOptions options);

        /// <summary>
        /// Returns a <see cref="ChannelBatchUpdater"/> instance for convenient batch channel operations.
        /// </summary>
        ChannelBatchUpdater BatchUpdater();

        /// <summary>
        /// Takes away moderators status from given user ids.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        Task<ApiResponse> DemoteModeratorsAsync(string channelType, string channelId, IEnumerable<string> userIds);

        /// <summary>
        /// <para>Mutes a channel.</para>
        /// Messages added to a muted channel will not trigger push notifications, nor change the
        /// unread count for the users that muted it. By default, mutes stay in place indefinitely
        /// until the user removes it; however, you can optionally set an expiration time. The list
        /// of muted channels and their expiration time is returned when the user connects.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/muting_channels/?language=csharp</remarks>
        Task<ChannelMuteResponse> MuteChannelAsync(ChannelMuteRequest request);

        /// <summary>
        /// <para>Unmutes a channel.</para>
        /// Messages added to a muted channel will not trigger push notifications, nor change the
        /// unread count for the users that muted it. By default, mutes stay in place indefinitely
        /// until the user removes it; however, you can optionally set an expiration time. The list
        /// of muted channels and their expiration time is returned when the user connects.
        /// This method removes the mute which means the users will receive notifications again.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/muting_channels/?language=csharp</remarks>
        Task<ChannelUnmuteResponse> UnmuteChannelAsync(ChannelUnmuteRequest request);

        /// <summary>
        /// <para>Export a channel and download all messages and metadata.</para>
        /// Channel exports are created asynchronously, you can use the Task ID returned by
        /// the APIs to keep track of the status and to download the final result when it is ready.
        /// Use <see cref="GetExportChannelsStatusAsync"/> to check the status of the export.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/exporting_channels/?language=csharp</remarks>
        Task<AsyncOperationResponse> ExportChannelAsync(ExportChannelItem request);

        /// <summary>
        /// <para>Export of one or many channels and download all messages and metadata.</para>
        /// Channel exports are created asynchronously, you can use the Task ID returned by
        /// the APIs to keep track of the status and to download the final result when it is ready.
        /// Use <see cref="GetExportChannelsStatusAsync"/> to check the status of the export.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/exporting_channels/?language=csharp</remarks>
        Task<AsyncOperationResponse> ExportChannelsAsync(ExportChannelRequest request);

        /// <summary>
        /// Returns the status of a channel export. It contains the URL to the JSON file.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/exporting_channels/?language=csharp</remarks>
        Task<ExportChannelsStatusResponse> GetExportChannelsStatusAsync(string taskId);

        /// <summary>
        /// Returns a channel. If it doesn't exist yet with the given name, it will be created.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/creating_channels/?language=csharp</remarks>
        Task<ChannelGetResponse> GetOrCreateAsync(string channelType, string channelId, string createdBy,
            params string[] members);

        /// <summary>
        /// Returns a channel. If it doesn't exist yet with the given name, it will be created.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/creating_channels/?language=csharp</remarks>
        Task<ChannelGetResponse> GetOrCreateAsync(string channelType, string channelId,
            ChannelGetRequest channelRequest);

        /// <summary>
        /// Returns a channel. If it doesn't exist yet with the given name, it will be created.
        /// This overload creates or gets a channel without explicit channel id.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/creating_channels/?language=csharp</remarks>
        Task<ChannelGetResponse> GetOrCreateAsync(string channelType, ChannelGetRequest channelRequest);

        /// <summary>
        /// <para>Removes a channel from query channel requests for that user until a new message is added.</para>
        /// Use <see cref="ShowAsync"/> to cancel this operation.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/muting_channels/?language=csharp</remarks>
        Task<ApiResponse> HideAsync(string channelType, string channelId, string userId, bool clearHistory = false);

        /// <summary>
        /// <para>Shows a previously hidden channel.</para>
        /// Use <see cref="HideAsync"/> to hide a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/muting_channels/?language=csharp</remarks>
        Task<ApiResponse> ShowAsync(string channelType, string channelId, string userId);

        /// <summary>
        /// Can be used to set and unset specific fields when it is necessary to retain additional custom data fields on the object.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_update/?language=csharp</remarks>
        Task<PartialUpdateChannelResponse> PartialUpdateAsync(string channelType, string channelId,
            PartialUpdateChannelRequest partialUpdateChannelRequest);

        /// <summary>
        /// <para>Queries channels.</para>
        /// You can query channels based on built-in fields as well as any custom field you add to channels.
        /// Multiple filters can be combined using AND, OR logical operators, each filter can use its
        /// comparison (equality, inequality, greater than, greater or equal, etc.).
        /// You can find the complete list of supported operators in the query syntax section of the docs.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/query_channels/?language=csharp</remarks>
        Task<QueryChannelResponse> QueryChannelsAsync(QueryChannelsOptions opts);

        /// <summary>
        /// <para>Queries members of a channel.</para>
        /// The queryMembers endpoint allows you to list and paginate members for a channel. The
        /// endpoint supports filtering on numerous criteria to efficiently return member information.
        /// This endpoint is useful for channels that have large lists of members and
        /// you want to search members or if you want to display the full list of members for a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/query_members/?language=csharp</remarks>
        Task<QueryMembersResponse> QueryMembersAsync(QueryMembersRequest queryMembersRequest);

        /// <summary>
        /// Removes member(s) from a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_members/?language=csharp</remarks>
        Task<ApiResponse> RemoveMembersAsync(string channelType, string channelId, IEnumerable<string> userIds,
            MessageRequest msg = null);

        /// <summary>
        /// Adds filter tags to a channel.
        /// </summary>
        Task<UpdateChannelResponse> AddFilterTagsAsync(string channelType, string channelId, IEnumerable<string> filterTags, MessageRequest msg = null);

        Task<UpdateChannelResponse> AddFilterTagsAsync(string channelType, string channelId, params string[] filterTags);

        /// <summary>
        /// Removes filter tags from a channel.
        /// </summary>
        Task<ApiResponse> RemoveFilterTagsAsync(string channelType, string channelId, IEnumerable<string> filterTags, MessageRequest msg = null);

        Task<ApiResponse> RemoveFilterTagsAsync(string channelType, string channelId, params string[] filterTags);

        /// <summary>
        /// Update channel member partially.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_member/#update-channel-members</remarks>
        Task<ChannelMemberResponse> UpdateMemberPartialAsync(string channelType, string channelId,
            ChannelMemberPartialRequest channelMemberPartialRequest);

        /// <summary>
        /// <para>Removes all messages but not affect the channel data or channel members.</para>
        /// If you want to delete both channel and message data then use <see cref="DeleteAsync"/> method instead.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/truncate_channel/?language=csharp</remarks>
        Task<TruncateResponse> TruncateAsync(string channelType, string channelId);

        /// <summary>
        /// <para>Removes all messages but not affect the channel data or channel members.</para>
        /// If you want to delete both channel and message data then use <see cref="DeleteAsync"/> method instead.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/truncate_channel/?language=csharp</remarks>
        Task<TruncateResponse> TruncateAsync(string channelType, string channelId, TruncateOptions truncateOptions);

        /// <summary>
        /// Invites a user to the channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_invites/?language=csharp/</remarks>
        Task<UpdateChannelResponse> InviteAsync(string channelType, string channelId, string userId,
            MessageRequest msg = null);

        /// <summary>
        /// Invites a user to the channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_invites/?language=csharp/</remarks>
        Task<UpdateChannelResponse> InviteAsync(string channelType, string channelId, IEnumerable<string> userIds);

        /// <summary>
        /// Invites a user to the channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_invites/?language=csharp/</remarks>
        Task<UpdateChannelResponse> InviteAsync(string channelType, string channelId, IEnumerable<string> userIds,
            MessageRequest msg = null);

        /// <summary>
        /// Accepts an invitaton to a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_invites/?language=csharp/</remarks>
        Task<UpdateChannelResponse> AcceptInviteAsync(string channelType, string channelId, string userId);

        /// <summary>
        /// Rejects an invitaton to a channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_invites/?language=csharp/</remarks>
        Task<UpdateChannelResponse> RejectInviteAsync(string channelType, string channelId, string userId);

        /// <summary>
        /// <para>Updates a channel.</para>
        /// The update function updates all channel data. Any data that is present on the channel
        /// and not included in a full update will be deleted. If you don't want that, use
        /// the <see cref="PartialUpdateAsync"/> method instead.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_update/?language=csharp</remarks>
        Task<UpdateChannelResponse> UpdateAsync(string channelType, string channelId,
            ChannelUpdateRequest updateRequest);

        /// <summary>
        /// Pins the channel for the user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_update/#pinning-a-channel</remarks>
        Task<ChannelMemberResponse> PinAsync(string channelType, string channelId, string userId);

        /// <summary>
        /// Unpins the channel for the user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_update/#pinning-a-channel</remarks>
        Task<ChannelMemberResponse> UnpinAsync(string channelType, string channelId, string userId);

        /// <summary>
        /// Archives the channel for the user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_update/#archiving-a-channel</remarks>
        Task<ChannelMemberResponse> ArchiveAsync(string channelType, string channelId, string userId);

        /// <summary>
        /// Unarchives the channel for the user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_update/#archiving-a-channel</remarks>
        Task<ChannelMemberResponse> UnarchiveAsync(string channelType, string channelId, string userId);
    }
}