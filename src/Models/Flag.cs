using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class FlagCreateRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "target_message_id")]
        public string TargetMessageId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "target_user_id")]
        public string TargetUserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }
    }

    public class FlagMessageQueryRequest : IQueryParameterConvertible
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "filter_conditions")]
        public Dictionary<string, object> FilterConditions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "limit")]
        public int Limit { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "offset")]
        public int Offset { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public UserRequest User { get; set; }

        public List<KeyValuePair<string, string>> ToQueryParameters()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("payload", JsonConvert.SerializeObject(this)),
            };
        }
    }

    public class Thresholds
    {
        public ModerationBehaviour Explicit { get; set; }

        public ModerationBehaviour Spam { get; set; }

        public ModerationBehaviour Toxic { get; set; }
    }

    public class MessageModerationResult
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message_id")]
        public string MessageId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "moderated_by")]
        public string ModeratedBy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocked_word")]
        public string BlockedWord { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocklist_name")]
        public string BlocklistName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "moderation_thresholds")]
        public Thresholds ModerationThresholds { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ai_moderation_response")]
        public ModerationScore AiModerationResponse { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_karma")]
        public int? UserKarma { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_bad_karma")]
        public bool? UserBadKarma { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class MessageFlag
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_by_automod")]
        public bool CreatedByAutomod { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "moderation_result")]
        public MessageModerationResult ModerationResult { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message")]
        public Message Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reviewed_at")]
        public DateTimeOffset? ReviewedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reviewed_by")]
        public User ReviewedBy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "approved_at")]
        public DateTimeOffset? ApprovedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "rejected_at")]
        public DateTimeOffset? RejectedAt { get; set; }
    }

    public class QueryMessageFlagsResponse : ApiResponse
    {
        public List<MessageFlag> Flags { get; set; }
    }
}