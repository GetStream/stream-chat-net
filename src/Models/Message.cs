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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mml")]
        public string MML { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "command")]
        public string Command { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "html")]
        public string HTML { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "silent")]
        public bool? Silent { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public UserRequest User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "latest_reactions")]
        public List<Reaction> LatestReactions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "own_reactions")]
        public List<Reaction> OwnReactions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reaction_counts")]
        public Dictionary<string, int> ReactionCounts { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reaction_scores")]
        public Dictionary<string, int> ReactionScores { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "parent_id")]
        public string ParentID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "shown_in_channel")]
        public bool? ShownInChannel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reply_count")]
        public int? ReplyCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "quoted_message_id")]
        public string QuotedMessageId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "thread_participants")]
        public List<User> ThreadParticipants { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "cid")]
        public string CID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mentioned_users")]
        public List<User> MentionedUsers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deleted_at")]
        public DateTimeOffset? DeletedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "shadowed")]
        public bool? Shadowed { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "image_labels")]
        public Dictionary<string, List<string>> ImageLabels { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "i18n")]
        public Dictionary<string, string> I18n { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "pinned")]
        public bool? Pinned { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "pin_expires")]
        public DateTimeOffset? PinExpires { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "pinned_by")]
        public UserRequest PinnedBy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "pinned_at")]
        public DateTimeOffset? PinnedAt { get; set; }
    }

    public class MessageSendRequest
    {
        public MessageRequest Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "skip_push")]
        public bool? SkipPush { get; set; }
    }

    public class MessageSearchResponse : ApiResponse
    {
        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("results")]
        public List<MessageSearchResult> Results { get; set; }
    }

    public class MessageSearchResult
    {
        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }
    }

    public class MessagePartialUpdateRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "set")]
        public Dictionary<string, object> Set { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "unset")]
        public IEnumerable<string> Unset { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "skip_enrich_url")]
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
