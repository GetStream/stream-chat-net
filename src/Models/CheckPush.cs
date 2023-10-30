using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamChat.Models
{
    public class AppCheckPushRequest
    {
        public string MessageId { get; set; }
        public string ApnTemplate { get; set; }
        public string FirebaseTemplate { get; set; }
        public string FirebaseDataTemplate { get; set; }
        public bool? SkipDevices { get; set; }
        public string PushProviderName { get; set; }
        public PushProviderType? PushProviderType { get; set; }
        public string UserId { get; set; }
        public UserRequest User { get; set; }
    }

    public class DeviceError
    {
        public string Provider { get; set; }
        public string ProviderName { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class AppCheckPushResponse : ApiResponse
    {
        public Dictionary<string, DeviceError> DeviceErrors { get; set; }
        public List<string> GeneralErrors { get; set; }
        public bool? SkipDevices { get; set; }
        public string RenderedApnTemplate { get; set; }
        public string RenderedFirebaseTemplate { get; set; }
        public Dictionary<string, string> RenderedMessage { get; set; }
    }

    public class AppCheckSqsRequest
    {
        public string SqsUrl { get; set; }
        public string SqsKey { get; set; }
        public string SqsSecret { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SqsCheckStatus
    {
        None,

        [EnumMember(Value = "ok")]
        Ok,

        [EnumMember(Value = "error")]
        Error,
    }

    public class AppCheckSqsResponse : ApiResponse
    {
        public SqsCheckStatus Status { get; set; }
        public string Error { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }

    public class AppCheckSnsRequest
    {
        public string SnsTopicArn { get; set; }
        public string SnsKey { get; set; }
        public string SnsSecret { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SnsCheckStatus
    {
        None,

        [EnumMember(Value = "ok")]
        Ok,

        [EnumMember(Value = "error")]
        Error,
    }

    public class AppCheckSnsResponse : ApiResponse
    {
        public SnsCheckStatus Status { get; set; }
        public string Error { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}