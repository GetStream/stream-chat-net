using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamChat.Models
{
    public class FileUploadConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "allowed_file_extensions")]
        public List<string> AllowedFileExtensions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocked_file_extensions")]
        public List<string> BlockedFileExtensions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "allowed_mime_types")]
        public List<string> AllowedMimeTypes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "blocked_mime_types")]
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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "disable_auth_checks")]
        public bool? DisableAuth { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "disable_permissions_checks")]
        public bool? DisablePermissions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "multi_tenant_enabled")]
        public bool? MultiTenantEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "permission_version")]
        public PermissionVersion? PermissionVersion { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "async_url_enrich_enabled")]
        public bool? AsyncURLEnrichEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "custom_action_handler_url")]
        public string CustomActionHandlerUrl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "webhook_url")]
        public string WebhookURL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "webhook_events")]
        public List<string> WebhookEvents { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_search_disallowed_roles")]
        public List<string> UserSearchDisallowedRoles { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "before_message_send_hook_url")]
        public string BeforeMessageSendHookUrl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "image_moderation_labels")]
        public List<string> ImageModerationLabels { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "image_moderation_enabled")]
        public bool? ImageModerationEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "auto_translation_enabled")]
        public bool? AutoTranslationEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "sqs_url")]
        public string SqsUrl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "sqs_key")]
        public string SqsKey { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "sqs_secret")]
        public string SqsSecret { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "image_upload_config")]
        public FileUploadConfig ImageUploadConfig { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "file_upload_config")]
        public FileUploadConfig FileUploadConfig { get; set; }

        [JsonProperty("revoke_tokens_issued_before")] // This can be null!
        public DateTimeOffset? RevokeTokensIssuedBefore { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "enforce_unique_usernames")]
        public UniqueUsernameEnforcementPolicy? EnforceUniqueUsernames { get; set; }
    }

    public class AppSettingsRequest : AppSettingsBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "apn_config")]
        public APNConfig APNConfig { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "firebase_config")]
        public FirebaseConfig FirebaseConfig { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "huawei_config")]
        public HuaweiConfig HuaweiConfig { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "migrate_permissions_to_v2")]
        public bool? MigratePermissionsToV2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "channel_hide_members_only")]
        public bool? ChannelHideMembersOnly { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "push_config")]
        public PushConfigRequest PushConfig { get; set; }
    }

    public class PushConfigRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "version")]
        public string Version { get; set; }
    }

    public static class APNAuthType
    {
        public const string Certificate = "certificate";
        public const string Token = "token";
    }

    public class APNConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "auth_type")]
        public string AuthType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "host")]
        public string Host { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "notification_template")]
        public string NotificationTemplate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "disabled")]
        public bool Disabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "auth_key")]
        public string AuthKey { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "development")]
        public bool? Development { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "key_id")]
        public string KeyID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "bundle_id")]
        public string BundleID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "team_id")]
        public string TeamID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "p12_cert")]
        public string P12Cert { get; set; }
    }

    public class FirebaseConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "server_key")]
        public string ServerKey { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "notification_template")]
        public string NotificationTemplate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "disabled")]
        public bool Disabled { get; set; }
    }

    public class HuaweiConfig
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "secret")]
        public string Secret { get; set; }
    }

    public class GetAppResponse : ApiResponse
    {
        public AppSettingsWithDetails App { get; set; }
    }
}
