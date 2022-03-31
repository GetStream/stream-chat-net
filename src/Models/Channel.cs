using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class Channel : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "cid")]
        public string Cid { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "team")]
        public string Team { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "config")]
        public ChannelTypeWithCommandsRequest Config { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_by")]
        public User CreatedBy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "frozen")]
        public bool Frozen { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "member_count")]
        public int MemberCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "members")]
        public List<ChannelMember> Members { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "messages")]
        public List<Message> Messages { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "read")]
        public List<ChannelRead> Read { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deleted_at")]
        public DateTimeOffset? DeletedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "last_message_at")]
        public DateTimeOffset? LastMessageAt { get; set; }

        public string TruncatedById { get; set; }
        public User TruncatedBy { get; set; }
    }

    public class ChannelWithConfig : Channel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "config")]
        public new ChannelConfigWithInfo Config { get; set; }
    }

    public class ChannelGetResponse : ApiResponse
    {
        [JsonProperty("channel")]
        public ChannelWithConfig Channel { get; set; }

        [JsonProperty("messages")]
        public List<Message> Messages { get; set; }

        [JsonProperty("pinned_messages")]
        public List<Message> PinnedMessages { get; set; }

        [JsonProperty("watcher_count")]
        public int WatcherCount { get; set; }

        [JsonProperty("watchers")]
        public List<User> Watchers { get; set; }

        [JsonProperty("read")]
        public List<ChannelRead> Reads { get; set; }

        [JsonProperty("members")]
        public List<ChannelMember> Members { get; set; }

        [JsonProperty("membership")]
        public ChannelMember Membership { get; set; }

        [JsonProperty("hidden")]
        public bool? Hidden { get; set; }

        [JsonProperty("hide_messages_before")]
        public DateTimeOffset? HideMessagesBefore { get; set; }
    }

    public class UpdateChannelResponse : ApiResponse
    {
        [JsonProperty("channel")]
        public ChannelWithConfig Channel { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }
    }

    public class PartialUpdateChannelRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "set")]
        public Dictionary<string, object> Set { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "unset")]
        public IEnumerable<string> Unset { get; set; }
    }

    public class PartialUpdateChannelResponse : ApiResponse
    {
        [JsonProperty("channel")]
        public ChannelWithConfig Channel { get; set; }

        [JsonProperty("members")]
        public List<ChannelMember> Members { get; set; }
    }

    public class TruncateOptions
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "hard_delete")]
        public bool? HardDelete { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message")]
        public MessageRequest Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "skip_push")]
        public bool? SkipPush { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "truncated_at")]
        public DateTimeOffset? TruncatedAt { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }

    public class TruncateResponse : ApiResponse
    {
        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("channel")]
        public ChannelWithConfig Channel { get; set; }
    }

    public class ChannelRequest : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_by")]
        public UserRequest CreatedBy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "team")]
        public string Team { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "auto_translation_enabled")]
        public bool? AutoTranslationEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "auto_translation_language")]
        public Language? AutoTranslationLanguage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "frozen")]
        public bool? Frozen { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "members")]
        public IEnumerable<ChannelMember> Members { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "config_overrides")]
        public ConfigOverridesRequest ConfigOverrides { get; set; }
    }

    public class QueryChannelResponse : ApiResponse
    {
        public List<ChannelGetResponse> Channels { get; set; }
    }

    public class QueryMembersResponse : ApiResponse
    {
        public List<ChannelMember> Members { get; set; }
    }

    public class AsyncOperationResponse : ApiResponse
    {
        [JsonProperty("task_id")]
        public string TaskId { get; set; }
    }

    public class ChannelUpdateRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "add_members")]
        public IEnumerable<string> AddMembers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "remove_members")]
        public IEnumerable<string> RemoveMembers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "add_moderators")]
        public IEnumerable<string> AddModerators { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "demote_moderators")]
        public IEnumerable<string> DemoteModerators { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "invites")]
        public IEnumerable<string> Invites { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "cooldown")]
        public int? Cooldown { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "accept_invite")]
        public bool? AcceptInvite { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reject_invite")]
        public bool? RejectInvite { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message")]
        public MessageRequest Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "skip_push")]
        public bool? SkipPush { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "hide_history")]
        public bool? HideHistory { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "data")]
        public ChannelRequest Data { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public UserRequest User { get; set; }
    }

    public class ChannelMuteRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "channel_cids")]
        public IEnumerable<string> ChannelCids { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "expiration")]
        public long? Expiration { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public UserRequest User { get; set; }
    }

    public class ChannelMuteResponse : ApiResponse
    {
        [JsonProperty(PropertyName = "channel_mute")]
        public ChannelMute ChannelMute { get; set; }

        [JsonProperty(PropertyName = "channel_mutes")]
        public List<ChannelMute> ChannelMutes { get; set; }

        [JsonProperty(PropertyName = "own_user")]
        public OwnUser OwnUser { get; set; }
    }

    public class ChannelUnmuteRequest : ChannelMuteRequest
    {
    }

    public class ChannelUnmuteResponse : ChannelMuteResponse
    {
    }
}
