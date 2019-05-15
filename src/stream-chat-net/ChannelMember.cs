using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public struct ChannelRole
    {
        public const string Member = "member";
        public const string Moderator = "moderator";
        public const string Admin = "admin";
        public const string Owner = "owner";
    }

    public class ChannelMember
    {
        public ChannelMember() { }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserID { get; internal set; }

        [JsonIgnore]
        public User User { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "is_moderator")]
        public bool IsModerator { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "invited")]
        public bool Invited { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "invite_accepted_at")]
        public DateTime? InviteAcceptedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "invite_rejected_at")]
        public DateTime? InviteRejectedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "role")]
        public string Role { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; internal set; }

        internal JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.User != null)
                root.Add("user", this.User.ToJObject());
            return root;
        }

        internal static ChannelMember FromJObject(JObject jObj)
        {
            var result = new ChannelMember();
            var data = JsonHelpers.FromJObject(result, jObj);
            var userObj = data.GetData<JObject>("user");
            if (userObj != null)
                result.User = User.FromJObject(userObj);

            return result;
        }
    }
}
