using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class MessageRequest : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mml")]
        public string Mml { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public UserRequest User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "parent_id")]
        public string ParentId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "attachments")]
        public IEnumerable<Attachment> Attachments { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "shown_in_channel")]
        public bool? ShownInChannel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mentioned_users")]
        public IEnumerable<string> MentionedUsers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "html")]
        public string HTML { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reaction_scores")]
        public Dictionary<string, int> ReactionScores { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "quoted_message_id")]
        public string QuotedMessageId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "cid")]
        public string Cid { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "silent")]
        public bool? Silent { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "pinned")]
        public bool? Pinned { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "pin_expires")]
        public DateTimeOffset? PinExpires { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "pinned_by")]
        public UserRequest PinnedBy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "pinned_at")]
        public DateTimeOffset? PinnedAt { get; set; }
    }
}
