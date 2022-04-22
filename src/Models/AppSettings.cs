using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamChat.Models
{
    public class FileUploadConfig
    {
        public List<string> AllowedFileExtensions { get; set; }
        public List<string> BlockedFileExtensions { get; set; }
        public List<string> AllowedMimeTypes { get; set; }
        public List<string> BlockedMimeTypes { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PermissionVersion
    {
        None,

        [EnumMember(Value = "v1")]
        V1,

        [EnumMember(Value = "v2")]
        V2,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UniqueUsernameEnforcementPolicy
    {
        None,

        [EnumMember(Value = "no")]
        No,

        [EnumMember(Value = "app")]
        App,

        [EnumMember(Value = "team")]
        Team,
    }

    public abstract class AppSettingsBase
    {
        [JsonProperty("disable_auth_checks")]
        public bool? DisableAuth { get; set; }

        [JsonProperty("disable_permissions_checks")]
        public bool? DisablePermissions { get; set; }

        public bool? MultiTenantEnabled { get; set; }
        public PermissionVersion? PermissionVersion { get; set; }
        public bool? AsyncUrlEnrichEnabled { get; set; }
        public string CustomActionHandlerUrl { get; set; }
        public string WebhookUrl { get; set; }
        public List<string> WebhookEvents { get; set; }
        public List<string> UserSearchDisallowedRoles { get; set; }
        public string BeforeMessageSendHookUrl { get; set; }
        public List<string> ImageModerationLabels { get; set; }
        public bool? ImageModerationEnabled { get; set; }
        public bool? AutoTranslationEnabled { get; set; }
        public int? RemindersInterval { get; set; }
        public string SqsUrl { get; set; }
        public string SqsKey { get; set; }
        public string SqsSecret { get; set; }
        public FileUploadConfig ImageUploadConfig { get; set; }
        public FileUploadConfig FileUploadConfig { get; set; }
        public DateTimeOffset? RevokeTokensIssuedBefore { get; set; }
        public UniqueUsernameEnforcementPolicy? EnforceUniqueUsernames { get; set; }
    }

    public class AppSettingsRequest : AppSettingsBase
    {
        public APNConfig ApnConfig { get; set; }
        public FirebaseConfig FirebaseConfig { get; set; }
        public HuaweiConfig HuaweiConfig { get; set; }
        public XiaomiConfig XiaomiConfig { get; set; }
        public bool? MigratePermissionsToV2 { get; set; }
        public bool? ChannelHideMembersOnly { get; set; }
        public PushConfigRequest PushConfig { get; set; }
    }

    public class PushConfigRequest
    {
        public string Version { get; set; }
        public bool? OfflineOnly { get; set; }
    }

    public static class APNAuthType
    {
        public const string Certificate = "certificate";
        public const string Token = "token";
    }

    public abstract class PushConfigBase
    {
        public bool Disabled { get; set; }
    }

    public class APNConfig : PushConfigBase
    {
        public string AuthType { get; set; }
        public string Host { get; set; }
        public string NotificationTemplate { get; set; }
        public string AuthKey { get; set; }
        public bool? Development { get; set; }
        public string KeyId { get; set; }
        public string BundleId { get; set; }
        public string TeamId { get; set; }

        [JsonProperty("p12_cert")]
        public string P12Cert { get; set; }
    }

    public class FirebaseConfig : PushConfigBase
    {
        public string ServerKey { get; set; }
        public string NotificationTemplate { get; set; }
        public string CredentialsJson { get; set; }
    }

    public class HuaweiConfig : PushConfigBase
    {
        public string Id { get; set; }
        public string Secret { get; set; }
    }

    public class XiaomiConfig : PushConfigBase
    {
        public string PackageName { get; set; }
        public string Secret { get; set; }
    }

    public class GetAppResponse : ApiResponse
    {
        public AppSettingsWithDetails App { get; set; }
    }
}
