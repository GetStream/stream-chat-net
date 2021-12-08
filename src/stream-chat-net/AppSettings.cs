using Newtonsoft.Json;

namespace StreamChat
{
    public class AppSettings
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "disable_auth_checks")]
        public bool DisableAuth { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "disable_permissions_checks")]
        public bool DisablePermissions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "multi_tenant_enabled")]
        public bool MultiTenantEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "async_url_enrich_enabled")]
        public bool AsyncURLEnrichEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "apn_config")]
        public APNConfig APNConfig { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "firebase_config")]
        public FirebaseConfig FirebaseConfig { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "webhook_url")]
        public string WebhookURL { get; set; }
    }

    public struct APNAuthType
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
}
