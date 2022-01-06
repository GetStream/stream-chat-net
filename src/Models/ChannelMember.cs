using System;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public static class ChannelRole
    {
        public const string Member = "member";
        public const string Moderator = "moderator";
        public const string Admin = "admin";
        public const string Owner = "owner";
    }

    public class ChannelMember
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public UserRequest User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "is_moderator")]
        public bool? IsModerator { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "invited")]
        public bool? Invited { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "invite_accepted_at")]
        public DateTimeOffset? InviteAcceptedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "invite_rejected_at")]
        public DateTimeOffset? InviteRejectedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "banned")]
        public bool? Banned { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ban_expires")]
        public DateTimeOffset? BanExpires { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "shadow_banned")]
        public bool? ShadowBanned { get; set; }
    }
}
