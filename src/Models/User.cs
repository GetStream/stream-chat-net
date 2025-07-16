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
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public Dictionary<string, string> TeamsRole { get; set; }
        public bool? Banned { get; set; }
        public DateTimeOffset? BanExpires { get; set; }
        public Language? Language { get; set; }
        public IEnumerable<string> Teams { get; set; }
    }

    public class User : CustomDataBase
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public Dictionary<string, string> TeamsRole { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? LastActive { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset? DeactivatedAt { get; set; }
        public bool? Online { get; set; }
        public string[] BlockedUserIds { get; set; }
        public float? AvgResponseTime { get; set; }
    }

    public class OwnUser
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public Dictionary<string, string> TeamsRole { get; set; }
        public List<string> Roles { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? LastActive { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset? DeactivatedAt { get; set; }
        public bool? Banned { get; set; }
        public bool? Online { get; set; }
        public bool? Invisible { get; set; }
        public List<Device> Devices { get; set; }
        public List<UserMute> Mutes { get; set; }
        public List<ChannelMute> ChannelMutes { get; set; }
        public int UnreadCount { get; set; }
        public int TotalUnreadCount { get; set; }
        public int UnreadChannels { get; set; }
        public Language? Language { get; set; }
        public List<string> Teams { get; set; }
        public List<string> LatestHiddenChannels { get; set; }
        public int? AvgResponseTime { get; set; }
    }

    public class AddMemberOptions
    {
        public bool? HideHistory { get; set; }
    }

    public class UserPartialRequest : CustomDataBase
    {
        public string Id { get; set; }
        public Dictionary<string, object> Set { get; set; }
        public IEnumerable<string> Unset { get; set; }
    }

    public class ChannelMemberPartialRequest
    {
        public string UserId { get; set; }
        public Dictionary<string, object> Set { get; set; }
        public IEnumerable<string> Unset { get; set; }
    }

    public class RoleAssignment
    {
        public string ChannelRole { get; set; }
        public string UserId { get; set; }
    }

    public class AssignRoleRequest
    {
        public List<RoleAssignment> AssignRoles { get; set; }
        public MessageRequest Message { get; set; }
    }

    public class GenericUserResponse : ApiResponse
    {
        public User User { get; set; }
    }

    public class CreateGuestResponse : GenericUserResponse
    {
        public string AccessToken { get; set; }
    }

    public class UploadSizeRequest
    {
        public string Crop { get; set; }
        public string Resize { get; set; }
        public int? Width { get; set; }
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
