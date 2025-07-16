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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum HookType
    {
        /// <summary>Standard HTTP webhook endpoint</summary>
        [EnumMember(Value = "webhook")]
        Webhook,

        /// <summary>Amazon SQS - managed message queuing service</summary>
        [EnumMember(Value = "sqs")]
        SQS,

        /// <summary>Amazon SNS - managed pub/sub messaging service</summary>
        [EnumMember(Value = "sns")]
        SNS,

        /// <summary>Hook for message review/modification before sending</summary>
        [EnumMember(Value = "pending_message")]
        PendingMessage,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthType
    {
        /// <summary>Authentication using access key and secret key pair</summary>
        [EnumMember(Value = "keys")]
        Keys,

        /// <summary>Authentication using IAM role-based access</summary>
        [EnumMember(Value = "role")]
        Role,

        /// <summary>Authentication using resource-based policies</summary>
        [EnumMember(Value = "resource")]
        Resource,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CallbackMode
    {
        [EnumMember(Value = "CALLBACK_MODE_NONE")]
        None,

        /// <summary>Use REST API for callbacks</summary>
        [EnumMember(Value = "CALLBACK_MODE_REST")]
        Rest,

        /// <summary>Use Twirp RPC protocol for callbacks</summary>
        [EnumMember(Value = "CALLBACK_MODE_TWIRP")]
        Twirp,
    }

    public class CallbackConfig
    {
        public CallbackMode Mode { get; set; }
    }

    public class EventHook
    {
        /// <summary>Unique identifier for the event hook</summary>
        public string Id { get; set; }

        /// <summary>Type of the hook (Webhook, SQS, SNS, or PendingMessage)</summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public HookType HookType { get; set; }

        /// <summary>Whether this event hook is currently active</summary>
        public bool Enabled { get; set; }

        /// <summary>List of event types this hook will respond to</summary>
        public List<string> EventTypes { get; set; }

        /// <summary>URL endpoint that will receive events</summary>
        public string WebhookUrl { get; set; }

        /// <summary>Amazon SQS queue URL for event delivery</summary>
        public string SqsQueueUrl { get; set; }

        /// <summary>AWS region for SQS queue</summary>
        public string SqsRegion { get; set; }

        /// <summary>Authentication type for SQS access</summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public AuthType SqsAuthType { get; set; }

        /// <summary>AWS access key ID for SQS</summary>
        public string SqsKey { get; set; }

        /// <summary>AWS secret access key for SQS</summary>
        public string SqsSecret { get; set; }

        /// <summary>IAM role ARN for SQS access</summary>
        public string SqsRoleArn { get; set; }

        /// <summary>SNS topic ARN for event publishing</summary>
        public string SnsTopicArn { get; set; }

        /// <summary>AWS region for SNS topic</summary>
        public string SnsRegion { get; set; }

        /// <summary>Authentication type for SNS access</summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public AuthType SnsAuthType { get; set; }

        /// <summary>AWS access key ID for SNS</summary>
        public string SnsKey { get; set; }

        /// <summary>AWS secret access key for SNS</summary>
        public string SnsSecret { get; set; }

        /// <summary>IAM role ARN for SNS access</summary>
        public string SnsRoleArn { get; set; }

        /// <summary>Maximum time in milliseconds to wait for hook processing</summary>
        public int TimeoutMs { get; set; }

        /// <summary>Configuration for callback behavior</summary>
        public CallbackConfig Callback { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
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
        public string SnsTopicArn { get; set; }
        public string SnsKey { get; set; }
        public string SnsSecret { get; set; }
        public FileUploadConfig ImageUploadConfig { get; set; }
        public FileUploadConfig FileUploadConfig { get; set; }
        public DateTimeOffset? RevokeTokensIssuedBefore { get; set; }
        public UniqueUsernameEnforcementPolicy? EnforceUniqueUsernames { get; set; }
        public Dictionary<string, List<string>> Grants { get; set; }
        public List<EventHook> EventHooks { get; set; }
        public bool? UserResponseTimeEnabled { get; set; }
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
        public bool? OfflineOnly { get; set; }
        public string Version { get; set; }
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
        public string ApnTemplate { get; set; }
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
