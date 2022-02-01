using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public static class Role
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

    public class UserRequest : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "banned")]
        public bool? Banned { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ban_expires")]
        public DateTimeOffset? BanExpires { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "language")]
        public Language? Language { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "teams")]
        public IEnumerable<string> Teams { get; set; }
    }

    public class User : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "last_active")]
        public DateTimeOffset? LastActive { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deleted_at")]
        public DateTimeOffset? DeletedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deactivated_at")]
        public DateTimeOffset? DeactivatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "online")]
        public bool? Online { get; set; }
    }

    public class OwnUser
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "roles")]
        public List<string> Roles { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "last_active")]
        public DateTimeOffset? LastActive { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deleted_at")]
        public DateTimeOffset? DeletedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deactivated_at")]
        public DateTimeOffset? DeactivatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "banned")]
        public bool? Banned { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "online")]
        public bool? Online { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "invisible")]
        public bool? Invisible { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "devices")]
        public List<Device> Devices { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mutes")]
        public List<UserMute> Mutes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "channel_mutes")]
        public List<ChannelMute> ChannelMutes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "unread_count")]
        public int UnreadCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "total_unread_count")]
        public int TotalUnreadCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "unread_channels")]
        public int UnreadChannels { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "language")]
        public Language? Language { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "teams")]
        public List<string> Teams { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "latest_hidden_channels")]
        public List<string> LatestHiddenChannels { get; set; }
    }

    public class AddMemberOptions
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "hide_history")]
        public bool? HideHistory { get; set; }
    }

    public class UserPartialRequest : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "set")]
        public Dictionary<string, object> Set { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "unset")]
        public IEnumerable<string> Unset { get; set; }
    }

    public class RoleAssignment
    {
        [JsonProperty(PropertyName = "channel_role")]
        public string ChannelRole { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }
    }

    public class AssignRoleRequest
    {
        [JsonProperty(PropertyName = "assign_roles")]
        public List<RoleAssignment> AssignRoles { get; set; }
        public MessageRequest Message { get; set; }
    }

    public class GenericUserResponse : ApiResponse
    {
        public User User { get; set; }
    }

    public class CreateGuestResponse : GenericUserResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
    }

    public class UploadSizeRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Crop { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Resize { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Width { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Height { get; set; }
    }

    public class UpsertResponse : ApiResponse
    {
        public Dictionary<string, User> Users { get; set; }
    }

    public class QueryUsersResponse : ApiResponse
    {
        public List<User> Users { get; set; }
    }

    public class QueryBannedUsersResponse : ApiResponse
    {
        public List<Ban> Bans { get; set; }
    }
}
