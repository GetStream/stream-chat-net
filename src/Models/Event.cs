using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public static class EventType
    {
        public const string UserPresenceChanged = "user.presence.changed";
        public const string UserWatchingStart = "user.watching.start";
        public const string UserWatchingStop = "user.watching.stop";
        public const string UserUpdated = "user.updated";
        public const string TypingStart = "typing.start";
        public const string TypingStop = "typing.stop";
        public const string MessageNew = "message.new";
        public const string MessageUpdated = "message.updated";
        public const string MessageDeleted = "message.deleted";
        public const string MessageRead = "message.read";
        public const string ReactionNew = "reaction.new";
        public const string ReactionDeleted = "reaction.deleted";
        public const string MemberAdded = "member.added";
        public const string MemberUpdated = "member.updated";
        public const string MemberRemoved = "member.removed";
        public const string ChannelUpdated = "channel.updated";
        public const string ChannelDeleted = "channel.deleted";
        public const string HealthCheck = "health.check";
        public const string NotificationNewMessage = "notification.message_new";
        public const string NotificationMarkRead = "notification.mark_read";
        public const string NotificationInvited = "notification.invited";
        public const string NotificationInviteAccepted = "notification.invite_accepted";
        public const string NotificationAddedToChannel = "notification.added_to_channel";
        public const string NotificationRemovedFromChannel = "notification.removed_from_channel";
        public const string NotificationMutesUpdated = "notification.mutes_updated";
    }

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
