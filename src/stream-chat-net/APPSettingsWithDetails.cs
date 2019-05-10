using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public class AppSettingsWithDetails
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "organization")]
        public string Organization { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "push_notifications")]
        public PushNotificationFields PushNotificationFields { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "webhook_url")]
        public string WebhookURL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "channel_configs")]
        public Dictionary<string, ChannelConfig> ChannelConfigs { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "suspended")]
        public bool Suspended { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "suspended_explanation")]
        public string SuspendedExplanation { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "disable_auth_checks")]
        public bool DisableAuth { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "disable_permissions_checks")]
        public bool DisablePermissions { get; set; }


        public AppSettingsWithDetails() { }
    }

    public class PushNotificationFields
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "apn")]
        public APNFields APN { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "firebase")]
        public FirebaseFields Firebase { get; set; }

        public PushNotificationFields() { }
    }

    public class APNFields
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "enabled")]
        public bool Enabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "auth_type")]
        public string AuthType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "notification_template")]
        public string NotificationTemplate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "host")]
        public string Host { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "bundle_id")]
        public string BundleID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "team_id")]
        public string TeamID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "key_id")]
        public string KeyID { get; set; }

        public APNFields() { }
    }

    public class FirebaseFields
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "notification_template")]
        public string NotificationTemplate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "enabled")]
        public bool Enabled { get; set; }

        public FirebaseFields() { }
    }
}
