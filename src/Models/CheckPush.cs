using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamChat.Models
{
    public class AppCheckPushRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message_id")]
        public string MessageId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "apn_template")]
        public string ApnTemplate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "firebase_template")]
        public string FirebaseTemplate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "firebase_data_template")]
        public string FirebaseDataTemplate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "skip_devices")]
        public bool? SkipDevices { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public UserRequest User { get; set; }
    }

    public class DeviceError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "provider")]
        public string Provider { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "error_message")]
        public string ErrorMessage { get; set; }
    }

    public class AppCheckPushResponse : ApiResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "device_errors")]
        public Dictionary<string, DeviceError> DeviceErrors { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "general_errors")]
        public List<string> GeneralErrors { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "skip_devices")]
        public bool? SkipDevices { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "rendered_apn_template")]
        public string RenderedApnTemplate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "rendered_firebase_template")]
        public string RenderedFirebaseTemplate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "rendered_message")]
        public Dictionary<string, string> RenderedMessage { get; set; }
    }

    public class AppCheckSqsRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "sqs_url")]
        public string SqsUrl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "sqs_key")]
        public string SqsKey { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "sqs_secret")]
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
}