using System;
using Newtonsoft.Json;


namespace StreamChat
{

    public class ExportChannelRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "messages_since")]
        public DateTime? MessagesSince { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "messages_until")]
        public DateTime? MessagesUntil { get; set; }

        public ExportChannelRequest() { }

        public ExportChannelRequest WithChannelType(string type)
        {
            this.Type = type;
            return this;
        }
        public ExportChannelRequest WithChannelId(string id)
        {
            this.Id = id;
            return this;
        }

        public ExportChannelRequest WithMessagesSince(DateTime since)
        {
            this.MessagesSince = since;
            return this;
        }

        public ExportChannelRequest WithMessagesUntil(DateTime until)
        {
            this.MessagesUntil = until;
            return this;
        }
    }

    public class ExportChannelsStatusResponse
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "task_id")]
        public string TaskId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "result")]
        public ExportChannelsResult Result { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class ExportChannelsResult
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "s3_bucket_name")]
        public string S3BucketName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "url")]
        public string URL { get; set; }
    }
}
