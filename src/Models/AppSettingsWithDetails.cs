using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class AppSettingsWithDetails : AppSettingsBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "organization")]
        public string Organization { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "push_notifications")]
        public PushNotificationFields PushNotificationFields { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "channel_configs")]
        public Dictionary<string, ChannelConfig> ChannelConfigs { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "suspended")]
        public bool Suspended { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "suspended_explanation")]
        public string SuspendedExplanation { get; set; }
    }

    public class PushNotificationFields
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "apn")]
        public APNFields APN { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "firebase")]
        public FirebaseFields Firebase { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "huawei")]
        public HuaweiFields Huawei { get; set; }
    }

    public abstract class PushNotificationBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "enabled")]
        public bool Enabled { get; set; }
    }

    public class APNFields : PushNotificationBase
    {
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
    }

    public class FirebaseFields : PushNotificationBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "notification_template")]
        public string NotificationTemplate { get; set; }
    }

    public class HuaweiFields : PushNotificationBase
    {
    }
}
