using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class ExportChannelRequest
    {
        public IEnumerable<ExportChannelItem> Channels { get; set; }
        public string Version { get; set; }
        public bool? ClearDeletedMessageText { get; set; }
        public bool? IncludeTruncatedMessages { get; set; }
        public bool? ExportUsers { get; set; }
    }

    public class ExportChannelItem
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public DateTimeOffset? MessagesSince { get; set; }
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
        public string TaskId { get; set; }
        public string Status { get; set; }
        public ExportChannelError Error { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public ExportChannelsResult Result { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class ExportChannelsResult
    {
        public string Path { get; set; }
        [JsonProperty("s3_bucket_name")]
        public string S3BucketName { get; set; }
        public string Url { get; set; }
    }
}
