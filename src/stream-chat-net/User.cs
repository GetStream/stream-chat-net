using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public struct Role
    {
        public const string Admin = "admin";
        public const string ChannelModerator = "channel_moderator";
        public const string ChannelMember = "channel_member";
        public const string User = "user";
        public const string Guest = "guest";
        public const string AnyAuthenticated = "any_authenticated";
        public const string Anonymous = "anonymous";
        public const string Any = "*";
    }

    public class User : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "last_active")]
        public DateTime? LastActive { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deleted_at")]
        public DateTime? DeletedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deactivated_at")]
        public DateTime? DeactivatedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "online")]
        public bool? Online { get; set; }


        public User() { }

        internal static User FromJObject(JObject jObj)
        {
            var result = new User();
            result._data = JsonHelpers.FromJObject(result, jObj);
            return result;
        }
    }
}
