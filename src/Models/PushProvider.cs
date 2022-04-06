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
        public DateTimeOffset DisabledAt { get; set; }
        public string DisabledReason { get; set; }
        public string ApnAuthKey { get; set; }
        public string ApnKeyId { get; set; }
        public string ApnTeamId { get; set; }
        public string ApnTopic { get; set; }
        public string FirebaseCredentials { get; set; }
        public string HuaweiAppId { get; set; }
        public string HuaweiAppSecret { get; set; }
        public string XiaomiPackageName { get; set; }
        public string XiaomiAppSecret { get; set; }
    }

    public class PushProvider : PushProviderRequest
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class UpsertPushProviderResponse : ApiResponse
    {
        public PushProvider PushProvider { get; set; }
    }

    public class ListPushProvidersResponse : ApiResponse
    {
        public List<PushProvider> PushProviders { get; set; }
    }
}