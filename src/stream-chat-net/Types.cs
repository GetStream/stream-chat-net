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
        [JsonProp(Tag = "id")]
        public string ID { get; set; }

        [JsonProp(Tag = "role")]
        public string Role { get; set; }

        [JsonProp(Tag = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProp(Tag = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProp(Tag = "last_active")]
        public DateTime? LastActive { get; set; }

        [JsonProp(Tag = "deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonProp(Tag = "deactivated_at")]
        public DateTime? DeactivatedAt { get; set; }

        [JsonProp(Tag = "online")]
        public bool? Online { get; set; }


        public User() { }

        internal static User FromJObject(JObject jObj)
        {
            var result = new User();
            result._data = JsonHelpers.FromJObject(ref result, jObj);
            return result;
        }
    }
}
