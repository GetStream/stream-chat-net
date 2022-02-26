using System;
using System.Collections.Generic;
using StreamChat.Utils;

namespace StreamChat.Models
{
    public class FlagCreateRequest
    {
        public string TargetMessageId { get; set; }
        public string TargetUserId { get; set; }
        public string UserId { get; set; }
    }

    public class FlagMessageQueryRequest : IQueryParameterConvertible
    {
        public Dictionary<string, object> FilterConditions { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public string UserId { get; set; }
        public UserRequest User { get; set; }

        public List<KeyValuePair<string, string>> ToQueryParameters()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("payload", StreamJsonConverter.SerializeObject(this)),
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
        public string MessageId { get; set; }
        public string Action { get; set; }
        public string ModeratedBy { get; set; }
        public string BlockedWord { get; set; }
        public string BlocklistName { get; set; }
        public Thresholds ModerationThresholds { get; set; }
        public ModerationScore AiModerationResponse { get; set; }
        public int? UserKarma { get; set; }
        public bool? UserBadKarma { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class MessageFlag
    {
        public bool CreatedByAutomod { get; set; }
        public MessageModerationResult ModerationResult { get; set; }
        public User User { get; set; }
        public Message Message { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? ReviewedAt { get; set; }
        public User ReviewedBy { get; set; }
        public DateTimeOffset? ApprovedAt { get; set; }
        public DateTimeOffset? RejectedAt { get; set; }
    }

    public class QueryMessageFlagsResponse : ApiResponse
    {
        public List<MessageFlag> Flags { get; set; }
    }
}