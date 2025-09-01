using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to retrieve, create and alter messages of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
    public interface IMessageClient
    {
        /// <summary>
        /// Commits a message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/pending_messages/?language=csharp</remarks>
        Task<GenericMessageResponse> CommitMessageAsync(string messageId);

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        Task<GenericMessageResponse> DeleteMessageAsync(string messageId);

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        Task<GenericMessageResponse> DeleteMessageAsync(string messageId, bool hardDelete);

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        Task<GenericMessageResponse> DeleteMessageAsync(string messageId, string deletedBy, bool hardDelete = false);

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        Task<GenericMessageResponse> DeleteMessageAsync(string messageId, string deletedBy, bool hardDelete = false, bool deleteForMe = false);

        /// <summary>
        /// Returns a message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        Task<GenericMessageResponse> GetMessageAsync(string messageId);

        /// <summary>
        /// Returns multiple messages.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        Task<GetMessagesResponse> GetMessagesAsync(string channelType, string channelId, IEnumerable<string> messageIds);

        /// <summary>
        /// <para>Pins a message.</para>
        /// Pinned messages allow users to highlight important messages, make announcements, or temporarily
        /// promote content. Pinning a message is, by default, restricted to certain user roles,
        /// but this is flexible. Each channel can have multiple pinned messages and these can be created
        /// or updated with or without an expiration.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/pinned_messages/?language=csharp</remarks>
        Task<MessagePartialUpdateResponse> PinMessageAsync(string messageId, string userId, TimeSpan? expiresIn = null);

        /// <summary>
        /// <para>Unpins a message.</para>
        /// Pinned messages allow users to highlight important messages, make announcements, or temporarily
        /// promote content. Pinning a message is, by default, restricted to certain user roles,
        /// but this is flexible. Each channel can have multiple pinned messages and these can be created
        /// or updated with or without an expiration.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/pinned_messages/?language=csharp</remarks>
        Task<MessagePartialUpdateResponse> UnpinMessageAsync(string messageId, string userId);

        /// <summary>
        /// <para>Translates a message.</para>
        /// This API endpoint translates an existing message to another language. The source language
        /// is inferred from the user language or detected automatically by analyzing its text.
        /// If possible it is recommended to store the user language. See the documentation.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/translation/?language=csharp</remarks>
        Task<GenericMessageResponse> TranslateMessageAsync(string messageId, Language language);

        /// <summary>
        /// Returns all replies of a given (parentId) message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/threads/?language=csharp</remarks>
        Task<GetMessagesResponse> GetRepliesAsync(string parentId);

        /// <summary>
        /// Returns all replies of a given (parentId) message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/threads/?language=csharp</remarks>
        Task<GetMessagesResponse> GetRepliesAsync(string parentId, MessagePaginationParams pagination);

        /// <summary>
        /// <para>Marks messages read.</para>
        /// The <paramref name="messageId"/> optional parameter is the ID of the message that is considered last read by client.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        Task<SendEventResponse> MarkReadAsync(string channelType, string channelId, string userId, string messageId = null);

        /// <summary>
        /// <para>MSearches for messages.</para>
        /// You can enable and/or disable the search indexing on a per channel
        /// type through the Stream Dashboard.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/search/?language=csharp</remarks>
        Task<MessageSearchResponse> SearchAsync(SearchOptions opts);

        /// <summary>
        /// <para>Sends a message to a thread in a channel.</para>
        /// <paramref name="parentId"/> is the ID of the parent message in the thread.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        Task<GenericMessageResponse> SendMessageToThreadAsync(string channelType, string channelId, MessageRequest msg, string userId, string parentId, bool skipPush = false);

        /// <summary>
        /// <para>Sends a message to a channel.</para>
        /// If you want to send a message to a thread, you can use <see cref="SendMessageToThreadAsync"/>.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        Task<GenericMessageResponse> SendMessageAsync(string channelType, string channelId, string userId, string text);

        /// <summary>
        /// <para>Sends a message to a channel.</para>
        /// If you want to send a message to a thread, you can use <see cref="SendMessageToThreadAsync"/>.
        /// <para>Supports force moderation via <see cref="SendMessageOptions.ForceModeration"/>.</para>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        Task<GenericMessageResponse> SendMessageAsync(string channelType, string channelId, MessageRequest msg, string userId, SendMessageOptions options);

        /// <summary>
        /// <para>Sends a message to a channel.</para>
        /// If you want to send a message to a thread, you can use <see cref="SendMessageToThreadAsync"/>.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp</remarks>
        Task<GenericMessageResponse> SendMessageAsync(string channelType, string channelId, MessageRequest msg, string userId, bool skipPush = false);

        /// <summary>
        /// <para>Fully overwrites a message.</para>
        /// For partial update, use <see cref="UpdateMessagePartialAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp#update-a-message</remarks>
        Task<GenericMessageResponse> UpdateMessageAsync(MessageRequest msg);

        /// <summary>
        /// <para>Partial updates a message.</para>
        /// A partial update can be used to set and unset specific fields when
        /// it is necessary to retain additional data fields on the object. AKA a patch style update.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_message/?language=csharp#partial-update</remarks>
        Task<MessagePartialUpdateResponse> UpdateMessagePartialAsync(string messageId, MessagePartialUpdateRequest partialUpdateRequest);

        /// <summary>
        /// <para>Uploads a file.</para>
        /// This functionality defaults to using the Stream CDN. If you would like, you can
        /// easily change the logic to upload to your own CDN of choice.
        /// </summary>
        /// <returns>The URL of the uploaded file.</returns>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/file_uploads/?language=csharp</remarks>
        Task<FileUrlResponse> UploadFileAsync(string channelType, string channelId, UserRequest user, byte[] file, string fileName);

        /// <summary>
        /// <para>Deletes a file.</para>
        /// This functionality defaults to using the Stream CDN. If you would like, you can
        /// easily change the logic to upload to your own CDN of choice.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/file_uploads/?language=csharp</remarks>
        Task<ApiResponse> DeleteFileAsync(string channelType, string channelId, string fileUrl);

        /// <summary>
        /// <para>Uploads an image.</para>
        /// Stream supported image types are: image/bmp, image/gif, image/jpeg, image/png, image/webp,
        /// image/heic, image/heic-sequence, image/heif, image/heif-sequence, image/svg+xml.
        /// You can set a more restrictive list for your application if needed.
        /// The maximum file size is 100MB.
        /// </summary>
        /// <returns>The URL of the uploaded file.</returns>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/file_uploads/?language=csharp#how-to-upload-a-file-or-image</remarks>
        Task<FileUrlResponse> UploadImageAsync(string channelType, string channelId, UserRequest user, byte[] file, string fileName);

        /// <summary>
        /// <para>Uploads an image.</para>
        /// Stream supported image types are: image/bmp, image/gif, image/jpeg, image/png, image/webp,
        /// image/heic, image/heic-sequence, image/heif, image/heif-sequence, image/svg+xml.
        /// You can set a more restrictive list for your application if needed.
        /// The maximum file size is 100MB.
        /// </summary>
        /// <returns>The URL of the uploaded file.</returns>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/file_uploads/?language=csharp#how-to-upload-a-file-or-image</remarks>
        Task<FileUrlResponse> UploadImageAsync(string channelType,
            string channelId,
            UserRequest user,
            byte[] file,
            string fileName,
            IEnumerable<UploadSizeRequest> uploadSizes = null);

        /// <summary>
        /// <para>Deletes an image.</para>
        /// Stream supported image types are: image/bmp, image/gif, image/jpeg, image/png, image/webp,
        /// image/heic, image/heic-sequence, image/heif, image/heif-sequence, image/svg+xml.
        /// You can set a more restrictive list for your application if needed.
        /// The maximum file size is 100MB.
        /// </summary>
        /// <returns>The URL of the uploaded file.</returns>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/file_uploads/?language=csharp#how-to-upload-a-file-or-image</remarks>
        Task<ApiResponse> DeleteImageAsync(string channelType, string channelId, string imageUrl);

        /// <summary>Runs a message command action.</summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/file_uploads/?language=csharp#how-to-upload-a-file-or-image</remarks>
        Task<GenericMessageResponse> RunMessageCommandActionAsync(string messageId, string userId, Dictionary<string, string> formData);

        /// <summary>
        /// Creates a reminder for a message.
        /// </summary>
        /// <param name="messageId">The ID of the message to create a reminder for</param>
        /// <param name="userId">The ID of the user creating the reminder</param>
        /// <param name="remindAt">When to remind the user (optional)</param>
        /// <returns>API response with the created reminder</returns>
        Task<ReminderResponse> CreateReminderAsync(string messageId, string userId, DateTime? remindAt = null);

        /// <summary>
        /// Updates a reminder for a message.
        /// </summary>
        /// <param name="messageId">The ID of the message with the reminder</param>
        /// <param name="userId">The ID of the user who owns the reminder</param>
        /// <param name="remindAt">When to remind the user (optional)</param>
        /// <returns>API response with the updated reminder</returns>
        Task<ReminderResponse> UpdateReminderAsync(string messageId, string userId, DateTime? remindAt = null);

        /// <summary>
        /// Deletes a reminder for a message.
        /// </summary>
        /// <param name="messageId">The ID of the message with the reminder</param>
        /// <param name="userId">The ID of the user who owns the reminder</param>
        /// <returns>API response</returns>
        Task<ReminderResponse> DeleteReminderAsync(string messageId, string userId);

        /// <summary>
        /// Queries reminders based on filter conditions.
        /// </summary>
        /// <param name="userId">The ID of the user whose reminders to query</param>
        /// <param name="filterConditions">Conditions to filter reminders</param>
        /// <param name="sort">Sort parameters (default: [{ field: 'remind_at', direction: 1 }])</param>
        /// <param name="options">Additional query options like limit, offset</param>
        /// <returns>API response with reminders</returns>
        Task<QueryRemindersResponse> QueryRemindersAsync(string userId, Dictionary<string, object> filterConditions = null, List<Dictionary<string, object>> sort = null, Dictionary<string, object> options = null);
    }
}