using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class ExportChannelRequest
    {
        [JsonProperty(PropertyName = "channels", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<ExportChannelItem> Channels { get; set; }

        [JsonProperty(PropertyName = "version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "clear_deleted_message_text", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ClearDeletedMessageText { get; set; }

        [JsonProperty(PropertyName = "include_truncated_messages", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IncludeTruncatedMessages { get; set; }

        [JsonProperty(PropertyName = "export_users", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ExportUsers { get; set; }
    }

    public class ExportChannelItem
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "messages_since")]
        public DateTimeOffset? MessagesSince { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "messages_until")]
        public DateTimeOffset? MessagesUntil { get; set; }

        public ExportChannelItem WithChannelType(string type)
        {
            Type = type;
            return this;
        }

        public ExportChannelItem WithChannelId(string id)
        {
            Id = id;
            return this;
        }

        public ExportChannelItem WithMessagesSince(DateTimeOffset since)
        {
            MessagesSince = since;
            return this;
        }

        public ExportChannelItem WithMessagesUntil(DateTimeOffset until)
        {
            MessagesUntil = until;
            return this;
        }
    }

    public class ExportChannelError
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string Stacktrace { get; set; }
        public string Version { get; set; }
    }

    public class ExportChannelsStatusResponse : ApiResponse
    {
        [JsonProperty("task_id")]
        public string TaskId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("error")]
        public ExportChannelError Error { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty("result")]
        public ExportChannelsResult Result { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class ExportChannelsResult
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "s3_bucket_name")]
        public string S3BucketName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "url")]
        public string Url { get; set; }
    }
}
