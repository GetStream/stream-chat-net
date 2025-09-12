using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to retrieve, create and alter users of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/rest/#tasks-gettask</remarks>
    public interface IUserClient
    {
        /// <summary>
        /// <para>Creates a JWT for a user.</para>
        /// <para>
        /// Stream uses JWT (JSON Web Tokens) to authenticate chat users, enabling them to login.
        /// Knowing whether a user is authorized to perform certain actions is managed
        /// separately via a role based permissions system.
        /// </para>
        /// By default, user tokens are valid indefinitely. You can set an <c>exp</c>
        /// or issued at claim by using the another overload.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/other-rest/tokens_and_authentication/</remarks>
        string CreateToken(string userId);

        /// <summary>
        /// <para>Creates a JWT for a user.</para>
        /// <para>
        /// Stream uses JWT (JSON Web Tokens) to authenticate chat users, enabling them to login.
        /// Knowing whether a user is authorized to perform certain actions is managed
        /// separately via a role based permissions system.
        /// </para>
        /// By default, user tokens are valid indefinitely. You can set an expiration
        /// to tokens by passing in an <paramref name="expiration"/> which is the 'exp' claim of the JWT.
        /// An optional <paramref name="issuedAt"/> can be passed in as well, which is the 'iat' claim of the JWT.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/other-rest/tokens_and_authentication/</remarks>
        string CreateToken(string userId, DateTimeOffset? expiration = null, DateTimeOffset? issuedAt = null);

        /// <summary>
        /// <para>Creates or updates users.</para>
        /// Any user present in the payload will have its data replaced with the new version.
        /// For partial updates, use <see cref="UpdatePartialAsync"/> method.
        /// You can send up to 100 users per API request in both upsert and partial update API.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/update_users/?language=csharp#server-side-user-updates-(batch)</remarks>
        Task<UpsertResponse> UpsertManyAsync(IEnumerable<UserRequest> users);

        /// <summary>
        /// <para>Creates or updates users.</para>
        /// Any user present in the payload will have its data replaced with the new version.
        /// For partial updates, use <see cref="UpdatePartialAsync"/> method.
        /// You can send up to 100 users per API request in both upsert and partial update API.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/update_users/?language=csharp#server-side-user-updates-(batch)</remarks>
        Task<UpsertResponse> UpsertAsync(UserRequest user);

        /// <summary>
        /// <para>Creates a guest user.</para>
        /// Guest sessions can be created client-side and do not require any server-side authentication.
        /// Support and livestreams are common use cases for guests users because really
        /// often you want a visitor to be able to use chat on your application without (or before)
        /// they have a regular user account.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/authless_users/?language=csharp</remarks>
        Task<CreateGuestResponse> CreateGuestAsync(UserRequest user);

        /// <summary>
        /// <para>Partial updates a user.</para>
        /// If you need to update a subset of properties for a user(s), you can use
        /// a partial update method. Both <see cref="UserPartialRequest.Set"/> and <see cref="UserPartialRequest.Unset"/> parameters can be provided to add, modify, or
        /// remove attributes to or from the target user(s). The set and unset parameters can be used separately or combined.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/update_users/?language=csharp#server-side-partial-update-(batch)</remarks>
        Task<UpsertResponse> UpdatePartialAsync(UserPartialRequest update);

        /// <summary>
        /// Revokes all tokens that belong to certain user.
        /// To revoke more than one user, use <see cref="RevokeManyUserTokensAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/tokens_and_authentication/?language=go#token-revocation-by-user</remarks>
        Task<UpsertResponse> RevokeUserTokenAsync(string userId, DateTimeOffset? issuedBefore);

        /// <summary>
        /// Revokes all tokens that belong to certain list of users.
        /// To revoke only a single user, use <see cref="RevokeUserTokenAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/tokens_and_authentication/?language=go#token-revocation-by-user</remarks>
        Task<UpsertResponse> RevokeManyUserTokensAsync(IEnumerable<string> userIds, DateTimeOffset? issuedBefore);

        /// <summary>
        /// <para>Partial updates multiple users.</para>
        /// If you need to update a subset of properties for a user(s), you can use
        /// a partial update method. Both <see cref="UserPartialRequest.Set"/> and <see cref="UserPartialRequest.Unset"/> parameters can be provided to add, modify, or
        /// remove attributes to or from the target user(s). The set and unset parameters can be used separately or combined.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/update_users/?language=csharp#server-side-partial-update-(batch)</remarks>
        Task<UpsertResponse> UpdateManyPartialAsync(IEnumerable<UserPartialRequest> updates);

        /// <summary>
        /// <para>Deletes a user.</para>
        /// <para>
        /// Once a user has been deleted, it cannot be un-deleted and the user_id cannot be used again.
        /// This method can only be called server-side due to security concerns, so please keep this in mind when attempting
        /// to make the call.
        /// After deleting a user, the user will no longer be able to Connect to Stream Chat, send or receive messages
        /// be displayed when querying users or have messages stored in Stream Chat
        /// (depending on whether or not <paramref name="markMessagesDeleted"/> is set to true or false).
        /// </para>
        /// The <paramref name="markMessagesDeleted"/> parameter is optional. This parameter will delete all messages
        /// associated with the user.
        /// If you would like to keep message history, ensure that <paramref name="markMessagesDeleted"/> is set to false.
        /// To perform a "hard delete" on the user, you must set <paramref name="markMessagesDeleted"/> to true and provide an additional
        /// parameter called <paramref name="hardDelete"/> with the value set to true. This method will delete all messages, reactions,
        /// and any other associated data with the user.
        /// Another option is <paramref name="deleteConversations"/>, if set true the deleted user is removed from all one-to-one channels.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/other-rest/update_users/#delete-a-user</remarks>
        Task<GenericUserResponse> DeleteAsync(string id, bool markMessagesDeleted = false, bool hardDelete = false, bool deleteConversations = false);

        /// <summary>
        /// <para>Deletes multiple users.</para>
        /// <para>
        /// It is an asynchronous operation. The returned task id can be polled with
        /// <see cref="ITaskClient.GetTaskStatusAsync"/> method.
        /// </para>
        /// Once a user has been deleted, it cannot be un-deleted and the user_id cannot be used again.
        /// This method can only be called server-side due to security concerns, so please keep this in mind when attempting
        /// to make the call. The <see cref="DeleteUsersRequest.MessageDeletionStrategy"/> parameter is optional.
        /// This parameter will delete all messages associated with the user.
        /// Another option is <see cref="DeleteUsersRequest.ConversationDeletionStrategy"/>, if set to Hard, the deleted user is removed from all one-to-one channels.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/other-rest/update_users/#delete-a-user</remarks>
        Task<GenericTaskIdResponse> DeleteManyAsync(DeleteUsersRequest request);

        /// <summary>
        /// <para>Deactivates a user.</para>
        /// Deactivated users cannot connect to Stream Chat, and can't send or receive messages.
        /// To reactivate a user, use <see cref="ReactivateAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/update_users/?language=csharp#deactivate-a-user</remarks>
        Task<GenericUserResponse> DeactivateAsync(string id, bool markMessagesDeleted = false, string createdById = null);

        /// <summary>
        /// <para>Reactivates a deactivated user.</para>
        /// To deactivate a user, use <see cref="DeactivateAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/update_users/?language=csharp#deactivate-a-user</remarks>
        Task<GenericUserResponse> ReactivateAsync(string id, bool restoreMessages = false, string name = null, string createdById = null);

        /// <summary>
        /// Exports a user and returns an object containing all of its data.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/update_users/?language=csharp#exporting-users</remarks>
        Task<ExportedUser> ExportAsync(string userId);

        /// <summary>
        /// Schedules user export task for a list of users
        /// </summary>
        /// <param name="userIds">user IDs to export</param>
        /// <returns>returns task ID that you can use to get export status (see <see cref="ITaskClient.GetTaskStatusAsync"/>)</returns>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/exporting_users/?language=csharp</remarks>
        Task<GenericTaskIdResponse> ExportUsersAsync(IEnumerable<string> userIds);

        /// <summary>
        /// <para>Shadow bans a user.</para>
        /// When a user is shadow banned, they will still be allowed to post messages,
        /// but any message sent during the will only be visible to the messages author
        /// and invisible to other users of the App.
        /// To remove a shadow ban, use <see cref="RemoveShadowBanAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp#ban</remarks>
        Task<ApiResponse> ShadowBanAsync(ShadowBanRequest shadowBanRequest);

        /// <summary>
        /// <para>Removes a shadow ban from a user.</para>
        /// When a user is shadow banned, they will still be allowed to post messages,
        /// but any message sent during the will only be visible to the messages author
        /// and invisible to other users of the App.
        /// To shadow ban a user, use <see cref="ShadowBanAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp#ban</remarks>
        Task<ApiResponse> RemoveShadowBanAsync(ShadowBanRequest shadowBanRequest);

        /// <summary>
        /// <para>Bans a user.</para>
        /// Users can be banned from an app entirely or from a channel.
        /// When a user is banned, they will not be allowed to post messages until the
        /// ban is removed or expired but will be able to connect to Chat and to channels as before.
        /// To unban a user, use <see cref="UnbanAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp#ban</remarks>
        Task<ApiResponse> BanAsync(BanRequest banRequest);

        /// <summary>
        /// <para>Unbans a user.</para>
        /// Users can be banned from an app entirely or from a channel.
        /// When a user is banned, they will not be allowed to post messages until the
        /// ban is removed or expired but will be able to connect to Chat and to channels as before.
        /// To ban a user, use <see cref="BanAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp#ban</remarks>
        Task<ApiResponse> UnbanAsync(BanRequest banRequest);

        /// <summary>
        /// <para>Queries banned users.</para>
        /// Banned users can be retrieved in different ways:
        /// 1) Using the dedicated query bans endpoint
        /// 2) User Search: you can add the banned:true condition to your search. Please note that
        /// this will only return users that were banned at the app-level and not the ones
        /// that were banned only on channels.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp#query-banned-users</remarks>
        Task<QueryBannedUsersResponse> QueryBannedUsersAsync(QueryBannedUsersRequest request);

        /// <summary>
        /// <para>Mutes a user.</para>
        /// Any user is allowed to mute another user. Mutes are stored at user level and returned with the
        /// rest of the user information when connectUser is called. A user will be be muted until the
        /// user is unmuted or the mute is expired.
        /// Use <see cref="UnmuteAsync"/> method to unmute a user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp#mutes</remarks>
        Task<MuteResponse> MuteAsync(string targetId, string id);

        /// <summary>
        /// <para>Unmutes a user.</para>
        /// Any user is allowed to mute another user. Mutes are stored at user level and returned with the
        /// rest of the user information when connectUser is called. A user will be be muted until the
        /// user is unmuted or the mute is expired.
        /// Use <see cref="MuteAsync"/> method to mute a user.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp#mutes</remarks>
        Task<ApiResponse> UnmuteAsync(string targetId, string id);

        /// <summary>
        /// Marks channels as read.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/rest/#channels-markchannelsread</remarks>
        Task<ApiResponse> MarkAllReadAsync(string id);

        /// <summary>
        /// <para>Allows you to search for users and see if they are online/offline.</para>
        /// You can filter and sort on the custom fields you've set for your user, the user id, and when the user was last active.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/query_users/?language=csharp/</remarks>
        Task<QueryUsersResponse> QueryAsync(QueryUserOptions opts);

        /// <summary>
        /// Block user from sending 1x1 messages
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/javascript/block_user/</remarks>
        Task<BlockUserResponse> BlockUserAsync(string targetId, string userID);

        /// <summary>
        /// Get list of blocked users by this user
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/javascript/block_user/</remarks>
        Task<GetBlockedUsersResponse> GetBlockedUsersAsync(string userID);

        /// <summary>
        /// Unblock user
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/javascript/block_user/</remarks>
        Task<ApiResponse> UnblockUserAsync(string targetId, string userID);

        /// <summary>
        /// Updates a user's live location.
        /// </summary>
        /// <param name="userID">The user ID</param>
        /// <param name="location">The location data to update</param>
        /// <returns>The server response containing the updated live location share, including channel CID, message ID, user ID, latitude, longitude, device ID, end time, creation and update timestamps, and duration.</returns>
        Task<SharedLocationResponse> UpdateUserLiveLocationAsync(string userID, SharedLocationRequest location);

        /// <summary>
        /// Gets all active live location shares for a user.
        /// </summary>
        /// <param name="userID">The user ID</param>
        /// <returns>The server response containing an array of active live location shares with details including channel CID, message ID, coordinates, device ID, and timestamps</returns>
        Task<ActiveLiveLocationsResponse> GetUserActiveLiveLocationsAsync(string userID);

        /// <summary>
        /// Marks messages as delivered for a user.
        /// Only works if the delivery_receipts setting is enabled for the user.
        /// Note: Unlike the JavaScript SDK, this method doesn't automatically check delivery receipts settings
        /// as the .NET SDK doesn't maintain user state. You should check this manually if needed.
        /// </summary>
        /// <param name="options">The mark delivered options containing channel and message IDs we want to mark as delivered</param>
        /// <returns>Event response or null if delivery receipts are disabled</returns>
        Task<EventResponse> MarkDeliveredAsync(MarkDeliveredOptions options);

        /// <summary>
        /// Convenience method to mark a message as delivered for a specific user.
        /// </summary>
        /// <param name="userID">The user ID</param>
        /// <param name="messageID">The message ID</param>
        /// <param name="channelCID">The channel CID (channel type:channel id)</param>
        /// <returns>Event response or null if delivery receipts are disabled</returns>
        Task<EventResponse> MarkDeliveredSimpleAsync(string userID, string messageID, string channelCID);
    }
}
