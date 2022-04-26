using System;
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
        public string Version { get; set; }
        public bool? OfflineOnly { get; set; }
        public APNFields APN { get; set; }
        public FirebaseFields Firebase { get; set; }
        public HuaweiFields Huawei { get; set; }
        public XiaomiFields Xiaomi { get; set; }
        public List<PushProviderConfig> Providers { get; set; }
    }

    public class PushProviderConfig
    {
        public string Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset DisabledAt { get; set; }
        public string DisabledReason { get; set; }
        public string Name { get; set; }
        public PushProviderType Type { get; set; }

        // Apn
        public string ApnAuthkey { get; set; }
        public string ApnAuthtype { get; set; }
        public bool ApnDevelopment { get; set; }
        public string ApnHost { get; set; }
        public string ApnKeyId { get; set; }
        public string ApnNotificationTemplate { get; set; }
        public string Apnp12Cert { get; set; }
        public string ApnTeamId { get; set; }
        public string ApnTopic { get; set; }

        // Firebase
        public string FirebaseApnTemplate { get; set; }
        public string FirebaseCredentials { get; set; }
        public string FirebaseDataTemplate { get; set; }
        public string FirebaseNotificationTemplate { get; set; }
        public string FirebaseServerKey { get; set; }

        // Huawei
        public string HuaweiAppId { get; set; }
        public string HuaweiAppSecret { get; set; }

        // Xiaomi
        public string XiaomiPackageName { get; set; }
        public string XiaomiSecret { get; set; }
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
        public string ApnTemplate { get; set; }
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
