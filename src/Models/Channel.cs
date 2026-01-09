using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StreamChat.Utils;

namespace StreamChat.Models
{
    public class Channel : CustomDataBase
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Cid { get; set; }
        public string Team { get; set; }
        public ChannelTypeWithCommandsRequest Config { get; set; }
        public User CreatedBy { get; set; }
        public bool Frozen { get; set; }
        public int MemberCount { get; set; }
        public int? MessageCount { get; set; }
        public List<ChannelMember> Members { get; set; }
        public List<Message> Messages { get; set; }
        public List<ChannelRead> Read { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset? LastMessageAt { get; set; }
        public User TruncatedBy { get; set; }
    }

    public class ChannelWithConfig : Channel
    {
        public new ChannelConfigWithInfo Config { get; set; }
    }

    public class ChannelGetResponse : ApiResponse
    {
        public ChannelWithConfig Channel { get; set; }
        public List<Message> Messages { get; set; }
        public List<Message> PinnedMessages { get; set; }

        public List<PendingMessage> PendingMessages { get; set; }
        public int WatcherCount { get; set; }
        public List<User> Watchers { get; set; }

        [JsonProperty("read")]
        public List<ChannelRead> Reads { get; set; }
        public List<ChannelMember> Members { get; set; }
        public ChannelMember Membership { get; set; }
        public bool? Hidden { get; set; }
        public DateTimeOffset? HideMessagesBefore { get; set; }
    }

    public class UpdateChannelResponse : ApiResponse
    {
        public ChannelWithConfig Channel { get; set; }
        public Message Message { get; set; }
        public List<ChannelMember> Members { get; set; }
    }

    public class PartialUpdateChannelRequest
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public Dictionary<string, object> Set { get; set; }
        public IEnumerable<string> Unset { get; set; }
    }

    public class PartialUpdateChannelResponse : ApiResponse
    {
        public ChannelWithConfig Channel { get; set; }
        public List<ChannelMember> Members { get; set; }
    }

    public class TruncateOptions
    {
        public bool? HardDelete { get; set; }
        public MessageRequest Message { get; set; }
        public bool? SkipPush { get; set; }
        public DateTimeOffset? TruncatedAt { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }

    public class TruncateResponse : ApiResponse
    {
        public Message Message { get; set; }
        public ChannelWithConfig Channel { get; set; }
    }

    public class ChannelRequest : CustomDataBase
    {
        public UserRequest CreatedBy { get; set; }
        public string Team { get; set; }
        public bool? AutoTranslationEnabled { get; set; }
        public Language? AutoTranslationLanguage { get; set; }
        public bool? Frozen { get; set; }
        public IEnumerable<ChannelMember> Members { get; set; }
        public IEnumerable<string> FilterTags { get; set; }
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
        public string TaskId { get; set; }
    }

    public class ChannelUpdateRequest
    {
        public IEnumerable<string> AddMembers { get; set; }
        public IEnumerable<string> RemoveMembers { get; set; }
        public IEnumerable<string> AddModerators { get; set; }
        public IEnumerable<string> DemoteModerators { get; set; }
        public IEnumerable<string> AddFilterTags { get; set; }
        public IEnumerable<string> RemoveFilterTags { get; set; }
        public IEnumerable<string> Invites { get; set; }
        public int? Cooldown { get; set; }
        public bool? AcceptInvite { get; set; }
        public bool? RejectInvite { get; set; }
        public MessageRequest Message { get; set; }
        public bool? SkipPush { get; set; }
        public bool? HideHistory { get; set; }
        public DateTimeOffset? HideHistoryBefore { get; set; }
        public ChannelRequest Data { get; set; }
        public string UserId { get; set; }
        public UserRequest User { get; set; }
    }

    public class ChannelMuteRequest
    {
        public IEnumerable<string> ChannelCids { get; set; }
        public long? Expiration { get; set; }
        public string UserId { get; set; }
        public UserRequest User { get; set; }
    }

    public class ChannelMuteResponse : ApiResponse
    {
        public ChannelMute ChannelMute { get; set; }
        public List<ChannelMute> ChannelMutes { get; set; }
        public OwnUser OwnUser { get; set; }
    }

    public class ChannelUnmuteRequest : ChannelMuteRequest
    {
    }

    public class ChannelUnmuteResponse : ChannelMuteResponse
    {
    }

    public enum ChannelBatchOperation
    {
        [EnumMember(Value = "addMembers")]
        AddMembers,

        [EnumMember(Value = "removeMembers")]
        RemoveMembers,

        [EnumMember(Value = "inviteMembers")]
        InviteMembers,

        [EnumMember(Value = "assignRoles")]
        AssignRoles,

        [EnumMember(Value = "addModerators")]
        AddModerators,

        [EnumMember(Value = "demoteModerators")]
        DemoteModerators,

        [EnumMember(Value = "hide")]
        Hide,

        [EnumMember(Value = "show")]
        Show,

        [EnumMember(Value = "archive")]
        Archive,

        [EnumMember(Value = "unarchive")]
        Unarchive,

        [EnumMember(Value = "updateData")]
        UpdateData,

        [EnumMember(Value = "addFilterTags")]
        AddFilterTags,

        [EnumMember(Value = "removeFilterTags")]
        RemoveFilterTags,
    }

    public class ChannelBatchMemberRequest
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("channel_role")]
        public string ChannelRole { get; set; }
    }

    public class ChannelDataUpdate
    {
        [JsonProperty("frozen")]
        public bool? Frozen { get; set; }

        [JsonProperty("disabled")]
        public bool? Disabled { get; set; }

        [JsonProperty("custom")]
        public Dictionary<string, object> Custom { get; set; }

        [JsonProperty("team")]
        public string Team { get; set; }

        [JsonProperty("config_overrides")]
        public Dictionary<string, object> ConfigOverrides { get; set; }

        [JsonProperty("auto_translation_enabled")]
        public bool? AutoTranslationEnabled { get; set; }

        [JsonProperty("auto_translation_language")]
        public string AutoTranslationLanguage { get; set; }
    }

    public class ChannelsBatchFilters
    {
        [JsonProperty("cids")]
        public object Cids { get; set; }

        [JsonProperty("types")]
        public object Types { get; set; }

        [JsonProperty("filter_tags")]
        public object FilterTags { get; set; }
    }

    public class ChannelsBatchOptions
    {
        [JsonProperty("operation")]
        [JsonConverter(typeof(Utils.EnumMemberStringEnumConverter))]
        public ChannelBatchOperation Operation { get; set; }

        [JsonProperty("filter")]
        public ChannelsBatchFilters Filter { get; set; }

        [JsonProperty("members")]
        public IEnumerable<ChannelBatchMemberRequest> Members { get; set; }

        [JsonProperty("data")]
        public ChannelDataUpdate Data { get; set; }

        [JsonProperty("filter_tags_update")]
        public IEnumerable<string> FilterTagsUpdate { get; set; }
    }
}
