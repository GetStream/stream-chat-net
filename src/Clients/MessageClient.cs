using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;
using StreamChat.Utils;
using HttpMethod = StreamChat.Rest.HttpMethod;

namespace StreamChat.Clients
{
    public class MessageClient : ClientBase, IMessageClient
    {
        internal MessageClient(IRestClient client) : base(client)
        {
        }

        public async Task<GenericMessageResponse> SendMessageToThreadAsync(string channelType, string channelId, MessageRequest msg, string userId, string parentId, bool skipPush = false)
        {
            msg.ParentId = parentId;
            return await SendMessageAsync(channelType, channelId, msg, userId, new SendMessageOptions { skipPush = skipPush });
        }

        public async Task<GenericMessageResponse> SendMessageAsync(string channelType, string channelId, string userId, string text)
            => await SendMessageAsync(channelType, channelId, new MessageRequest { Text = text }, userId, new SendMessageOptions {});

        public async Task<GenericMessageResponse> SendMessageAsync(string channelType, string channelId, MessageRequest msg, string userId, bool skipPush = false)
            => await SendMessageAsync(channelType, channelId, msg, userId, new SendMessageOptions { skipPush = skipPush });
        public async Task<GenericMessageResponse> SendMessageAsync(string channelType, string channelId, MessageRequest msg, string userId, SendMessageOptions options)
        {
            var req = new MessageSendRequest
            {
                Message = msg,
                SkipPush = options.skipPush,
                IsPendingMessage = options.isPendingMessage,
            };
            req.Message.UserId = userId;

            return await ExecuteRequestAsync<GenericMessageResponse>($"channels/{channelType}/{channelId}/message",
                HttpMethod.POST,
                HttpStatusCode.Created,
                req);
        }

        public async Task<SendEventResponse> MarkReadAsync(string channelType, string channelId, string userId, string messageId = null)
            => await ExecuteRequestAsync<SendEventResponse>($"channels/{channelType}/{channelId}/read",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new
                {
                    user = new
                    {
                        id = userId,
                    },
                    message_id = messageId,
                });

        public async Task<MessageSearchResponse> SearchAsync(SearchOptions opts)
            => await ExecuteRequestAsync<MessageSearchResponse>("search",
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: opts.ToQueryParameters());

        public async Task<GenericMessageResponse> UpdateMessageAsync(MessageRequest msg)
            => await ExecuteRequestAsync<GenericMessageResponse>($"messages/{msg.Id}",
                HttpMethod.POST,
                HttpStatusCode.Created,
                body: new { message = msg });

        public async Task<MessagePartialUpdateResponse> UpdateMessagePartialAsync(string messageId, MessagePartialUpdateRequest partialUpdateRequest)
            => await ExecuteRequestAsync<MessagePartialUpdateResponse>($"messages/{messageId}",
                HttpMethod.PUT,
                HttpStatusCode.Created,
                partialUpdateRequest);

        public async Task<GenericMessageResponse> DeleteMessageAsync(string messageId, bool hardDelete = false)
            => await ExecuteRequestAsync<GenericMessageResponse>($"messages/{messageId}",
                HttpMethod.DELETE,
                HttpStatusCode.OK,
                queryParams: new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("hard", hardDelete.ToString().ToLowerInvariant()),
                });

        public async Task<GenericMessageResponse> GetMessageAsync(string messageId)
            => await ExecuteRequestAsync<GenericMessageResponse>($"messages/{messageId}",
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<GetMessagesResponse> GetMessagesAsync(string channelType, string channelId, IEnumerable<string> messageIds)
            => await ExecuteRequestAsync<GetMessagesResponse>($"/channels/{channelType}/{channelId}/messages",
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("ids", string.Join(",", messageIds)),
                });

        public async Task<MessagePartialUpdateResponse> PinMessageAsync(string messageId, string userId, TimeSpan? expiresIn = null)
            => await UpdateMessagePartialAsync(messageId, new MessagePartialUpdateRequest
            {
                Set = new Dictionary<string, object>
                    {
                        { "pinned", true },
                        { "pin_expires", expiresIn?.TotalSeconds },
                    },
                UserId = userId,
            });

        public async Task<MessagePartialUpdateResponse> UnpinMessageAsync(string messageId, string userId)
            => await UpdateMessagePartialAsync(messageId, new MessagePartialUpdateRequest
            {
                Set = new Dictionary<string, object>
                {
                    { "pinned", false },
                },
                UserId = userId,
            });

        public async Task<GenericMessageResponse> TranslateMessageAsync(string messageId, Language language)
            => await ExecuteRequestAsync<GenericMessageResponse>($"messages/{messageId}/translate",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new { language });

        public async Task<GetMessagesResponse> GetRepliesAsync(string parentId)
            => await GetRepliesAsync(parentId, null);

        public async Task<GetMessagesResponse> GetRepliesAsync(string parentId, MessagePaginationParams pagination)
            => await ExecuteRequestAsync<GetMessagesResponse>($"messages/{parentId}/replies",
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: pagination?.ToQueryParameters());

        public async Task<FileUrlResponse> UploadFileAsync(string channelType, string channelId, UserRequest user, byte[] file, string fileName)
        {
            var body = new MultipartFormDataContent();
            body.Add(new ByteArrayContent(file), "file", fileName);
            body.Add(new StringContent(StreamJsonConverter.SerializeObject(user)), "user");

            return await ExecuteRequestAsync<FileUrlResponse>($"channels/{channelType}/{channelId}/file",
                HttpMethod.POST,
                HttpStatusCode.Created,
                multipartBody: body);
        }

        public async Task<ApiResponse> DeleteFileAsync(string channelType, string channelId, string fileUrl)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}/file",
                HttpMethod.DELETE,
                HttpStatusCode.OK,
                queryParams: new Dictionary<string, string>
                {
                    { "url", fileUrl },
                });

        public async Task<FileUrlResponse> UploadImageAsync(string channelType, string channelId, UserRequest user, byte[] file, string fileName)
            => await UploadImageAsync(channelType, channelId, user, file, fileName, null);

        public async Task<FileUrlResponse> UploadImageAsync(string channelType,
            string channelId,
            UserRequest user,
            byte[] file,
            string fileName,
            IEnumerable<UploadSizeRequest> uploadSizes = null)
        {
            var body = new MultipartFormDataContent();
            body.Add(new ByteArrayContent(file), "file", fileName);
            body.Add(new StringContent(StreamJsonConverter.SerializeObject(user)), "user");

            if (uploadSizes != null)
            {
                body.Add(new StringContent(StreamJsonConverter.SerializeObject(uploadSizes)), "upload_sizes");
            }

            return await ExecuteRequestAsync<FileUrlResponse>($"channels/{channelType}/{channelId}/image",
                HttpMethod.POST,
                HttpStatusCode.Created,
                multipartBody: body);
        }

        public async Task<ApiResponse> DeleteImageAsync(string channelType, string channelId, string imageUrl)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}/image",
                HttpMethod.DELETE,
                HttpStatusCode.OK,
                queryParams: new Dictionary<string, string>
                {
                    { "url", imageUrl },
                });

        public async Task<GenericMessageResponse> RunMessageCommandActionAsync(string messageId, string userId, Dictionary<string, string> formData)
            => await ExecuteRequestAsync<GenericMessageResponse>($"/messages/{messageId}/action",
                HttpMethod.POST,
                HttpStatusCode.Created,
                body: new { user_id = userId, form_data = formData });

        public async Task<GenericMessageResponse> CommitMessageAsync(string messageId)
            => await ExecuteRequestAsync<GenericMessageResponse>($"/messages/{messageId}/commit",
                HttpMethod.POST,
                HttpStatusCode.Created);
    }
}