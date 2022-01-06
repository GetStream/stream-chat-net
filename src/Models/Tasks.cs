using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamChat.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeletionStrategy
    {
        None,

        [EnumMember(Value = "soft")]
        Soft,

        [EnumMember(Value = "pruning")]
        Pruning,

        [EnumMember(Value = "hard")]
        Hard,
    }

    public class DeleteUsersRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_ids")]
        public IEnumerable<string> UserIds { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public DeletionStrategy? UserDeletionStrategy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "messages")]
        public DeletionStrategy? MessageDeletionStrategy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "conversations")]
        public DeletionStrategy? ConversationDeletionStrategy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "new_channel_owner_id")]
        public string NewChannelOwnerId { get; set; }

        /// <summary>
        /// List of user ids do delete.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/other-rest/update_users/#delete-a-user</remarks>
        public DeleteUsersRequest WithUserIds(IEnumerable<string> userIds)
        {
            UserIds = userIds;
            return this;
        }

        /// <summary>
        /// List of user ids do delete.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/other-rest/update_users/#delete-a-user</remarks>
        public DeleteUsersRequest WithUserIds(params string[] userIds)
        {
            UserIds = userIds;
            return this;
        }

        /// <summary>
        /// Deletion strategy for user deletion.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/other-rest/update_users/#delete-a-user</remarks>
        public DeleteUsersRequest WithUserDeletionStrategy(DeletionStrategy strategy)
        {
            UserDeletionStrategy = strategy;
            return this;
        }

        /// <summary>
        /// Deletion strategy for message deletion.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/other-rest/update_users/#delete-a-user</remarks>
        public DeleteUsersRequest WithMessagesDeletionStrategy(DeletionStrategy strategy)
        {
            MessageDeletionStrategy = strategy;
            return this;
        }

        /// <summary>
        /// Deletion strategy for conversation deletion.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/other-rest/update_users/#delete-a-user</remarks>
        public DeleteUsersRequest WithConversationsDeletionStrategy(DeletionStrategy strategy)
        {
            ConversationDeletionStrategy = strategy;
            return this;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AsyncTaskStatus
    {
        None,

        [EnumMember(Value = "waiting")]
        Waiting,

        [EnumMember(Value = "pending")]
        Pending,

        [EnumMember(Value = "running")]
        Running,

        [EnumMember(Value = "completed")]
        Completed,

        [EnumMember(Value = "failed")]
        Failed,
    }

    public class GenericTaskIdResponse : ApiResponse
    {
        [JsonProperty(PropertyName = "task_id")]
        public string TaskId { get; set; }
    }

    public class AsyncTaskStatusResponse : GenericTaskIdResponse
    {
        public AsyncTaskStatus Status { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        public Dictionary<string, object> Result { get; set; }

        [JsonProperty(PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
