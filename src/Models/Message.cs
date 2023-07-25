using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public static class MessageType
    {
        public const string Regular = "regular";
        public const string Ephemeral = "ephemeral";
        public const string Error = "error";
        public const string Reply = "reply";
        public const string System = "system";
    }

    public class Message : CustomDataBase
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string MML { get; set; }
        public string Command { get; set; }
        public string HTML { get; set; }
        public string Type { get; set; }
        public bool? Silent { get; set; }
        public UserRequest User { get; set; }
        public List<Attachment> Attachments { get; set; }
        public List<Reaction> LatestReactions { get; set; }
        public List<Reaction> OwnReactions { get; set; }
        public Dictionary<string, int> ReactionCounts { get; set; }
        public Dictionary<string, int> ReactionScores { get; set; }
        public string ParentId { get; set; }
        public bool? ShownInChannel { get; set; }
        public int? ReplyCount { get; set; }
        public string QuotedMessageId { get; set; }
        public List<User> ThreadParticipants { get; set; }
        public string CID { get; set; }
        public List<User> MentionedUsers { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public bool? Shadowed { get; set; }
        public Dictionary<string, List<string>> ImageLabels { get; set; }

        [JsonProperty("i18n")]
        public Dictionary<string, string> I18n { get; set; }
        public bool? Pinned { get; set; }
        public DateTimeOffset? PinExpires { get; set; }
        public UserRequest PinnedBy { get; set; }
        public DateTimeOffset? PinnedAt { get; set; }
    }

    public class PendingMessage : CustomDataBase
    {
        public Message Message { get; set; }
        public Dictionary<string, object> PendingMessageMetadata { get; set; }
    }

    public class MessageSendRequest
    {
        public MessageRequest Message { get; set; }

        public bool? SkipPush { get; set; }
        public bool? IsPendingMessage { get; set; }
    }

    public class MessageSearchResponse : ApiResponse
    {
        public string Next { get; set; }
        public string Previous { get; set; }
        public List<MessageSearchResult> Results { get; set; }
    }

    public class MessageSearchResult
    {
        public Message Message { get; set; }
        public Channel Channel { get; set; }
    }

    public class MessagePartialUpdateRequest
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public Dictionary<string, object> Set { get; set; }
        public IEnumerable<string> Unset { get; set; }
        public bool? SkipEnrichUrl { get; set; }
    }

    public class GenericMessageResponse : ApiResponse
    {
        public Message Message { get; set; }
    }

    public class MessagePartialUpdateResponse : GenericMessageResponse
    {
    }

    public class GetMessagesResponse : ApiResponse
    {
        public List<Message> Messages { get; set; }
    }

    public class FileUrlResponse : ApiResponse
    {
        public string File { get; set; }
    }
}
