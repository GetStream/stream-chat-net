using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamChat.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PushProviderType
    {
        None,

        [EnumMember(Value = "firebase")]
        Firebase,

        [EnumMember(Value = "apn")]
        Apn,

        [EnumMember(Value = "huawei")]
        Huawei,

        [EnumMember(Value = "xiaomi")]
        Xiaomi,
    }

    public class PushProviderRequest
    {
        public PushProviderType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonProperty("disabled_at")]
        public DateTimeOffset DisabledAt { get; set; }
        [JsonProperty("disabled_reason")]
        public string DisabledReason { get; set; }

        [JsonProperty("apn_auth_key")]
        public string ApnAuthKey { get; set; }
        [JsonProperty("apn_key_id")]
        public string ApnKeyId { get; set; }
        [JsonProperty("apn_team_id")]
        public string ApnTeamId { get; set; }
        [JsonProperty("apn_topic")]
        public string ApnTopic { get; set; }

        [JsonProperty("firebase_credentials")]
        public string FirebaseCredentials { get; set; }

        [JsonProperty("huawei_app_id")]
        public string HuaweiAppId { get; set; }
        [JsonProperty("huawei_app_secret")]
        public string HuaweiAppSecret { get; set; }

        [JsonProperty("xiaomi_package_name")]
        public string XiaomiPackageName { get; set; }
        [JsonProperty("xiaomi_app_secret")]
        public string XiaomiAppSecret { get; set; }
    }

    public class PushProvider : PushProviderRequest
    {
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class UpsertPushProviderResponse : ApiResponse
    {
        [JsonProperty("push_provider")]
        public PushProvider PushProvider { get; set; }
    }

    public class ListPushProvidersResponse : ApiResponse
    {
        [JsonProperty("push_providers")]
        public List<PushProvider> PushProviders { get; set; }
    }
}