using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class AppSettingsWithDetails : AppSettingsBase
    {
        public string Name { get; set; }
        public string Organization { get; set; }

        [JsonProperty("push_notifications")]
        public PushNotificationFields PushNotificationFields { get; set; }
        public Dictionary<string, ChannelConfig> ChannelConfigs { get; set; }
        public bool Suspended { get; set; }
        public string SuspendedExplanation { get; set; }
    }

    public class PushNotificationFields
    {
        public APNFields APN { get; set; }
        public FirebaseFields Firebase { get; set; }
        public HuaweiFields Huawei { get; set; }
        public XiaomiFields Xiaomi { get; set; }
    }

    public abstract class PushNotificationBase
    {
        public bool Enabled { get; set; }
    }

    public class APNFields : PushNotificationBase
    {
        public string AuthType { get; set; }
        public string NotificationTemplate { get; set; }
        public string Host { get; set; }
        public string BundleId { get; set; }
        public string TeamId { get; set; }
        public string KeyId { get; set; }
    }

    public class FirebaseFields : PushNotificationBase
    {
        public string NotificationTemplate { get; set; }
        public string DataTemplate { get; set; }

    }

    public class HuaweiFields : PushNotificationBase
    {
        public string Id { get; set; }
        public string Secret { get; set; }
    }

    public class XiaomiFields : PushNotificationBase
    {
        public string PackageName { get; set; }
        public string Secret { get; set; }
    }
}
