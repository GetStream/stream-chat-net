using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public struct EventType
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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "connection_id")]
        public string ConnectionID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "cid")]
        public string CID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "watcher_count")]
        public int WatcherCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonIgnore]
        public Message Message { get; set; }

        [JsonIgnore]
        public Reaction Reaction { get; set; }

        [JsonIgnore]
        public ChannelObject Channel { get; set; }

        [JsonIgnore]
        public ChannelMember Member { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public User OwnUser { get; set; }

        public Event() { }

        public new JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.Message != null)
                root.Add("message", this.Message.ToJObject());
            if (this.Reaction != null)
                root.Add("reaction", this.Reaction.ToJObject());
            if (this.Channel != null)
                root.Add("channel", this.Channel.ToJObject());
            if (this.Member != null)
                root.Add("member", this.Member.ToJObject());
            if (this.User != null)
                root.Add("user", this.User.ToJObject());
            if (this.OwnUser != null)
                root.Add("me", this.OwnUser.ToJObject());

            this._data.AddToJObject(root);
            return root;
        }

        public static Event FromJObject(JObject jObj)
        {
            var result = new Event();
            result._data = JsonHelpers.FromJObject(result, jObj);

            var msgObj = result._data.GetData<JObject>("message");
            if (msgObj != null)
            {
                result.Message = Message.FromJObject(msgObj);
                result._data.RemoveData("message");
            }
            var reactionObj = result._data.GetData<JObject>("reaction");
            if (reactionObj != null)
            {
                result.Reaction = Reaction.FromJObject(reactionObj);
                result._data.RemoveData("reaction");
            }
            var chanObj = result._data.GetData<JObject>("channel");
            if (chanObj != null)
            {
                result.Channel = ChannelObject.FromJObject(chanObj);
                result._data.RemoveData("channel");
            }
            var memberObj = result._data.GetData<JObject>("member");
            if (memberObj != null)
            {
                result.Member = ChannelMember.FromJObject(memberObj);
                result._data.RemoveData("member");
            }
            var userObj = result._data.GetData<JObject>("user");
            if (userObj != null)
            {
                result.User = User.FromJObject(userObj);
                result._data.RemoveData("user");
            }
            userObj = result._data.GetData<JObject>("me");
            if (userObj != null)
            {
                result.OwnUser = User.FromJObject(userObj);
                result._data.RemoveData("me");
            }

            return result;
        }
    }
}
