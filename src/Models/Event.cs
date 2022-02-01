using System;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class Event : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "connection_id")]
        public string ConnectionID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "cid")]
        public string Cid { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "channel_type")]
        public string ChannelType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message")]
        public MessageRequest Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reaction")]
        public Reaction Reaction { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "channel")]
        public ChannelRequest Channel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "member")]
        public ChannelMember Member { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public UserRequest User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "me")]
        public OwnUser Me { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "watcher_count")]
        public int? WatcherCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reason")]
        public string Reason { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_by")]
        public User CreatedBy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "auto_moderation")]
        public bool? AutoModeration { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "automoderation_scores")]
        public ModerationScore AutomoderationScores { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "parent_id")]
        public string ParentId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "team")]
        public string Team { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }
    }

    public class SendEventResponse : ApiResponse
    {
        public Event Event { get; set; }
    }

    public class ModerationScore
    {
        public int Toxic { get; set; }

        public int Explicit { get; set; }

        public int Spam { get; set; }
    }
}
