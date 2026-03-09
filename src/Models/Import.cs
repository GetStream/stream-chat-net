using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StreamChat.Utils;

namespace StreamChat.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImportMode
    {
        None,

        [EnumMember(Value = "upsert")]
        Upsert,

        [EnumMember(Value = "insert")]
        Insert,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImportState
    {
        None,

        [EnumMember(Value = "queued")]
        Queued,

        [EnumMember(Value = "uploaded")]
        Uploaded,

        [EnumMember(Value = "analyzing")]
        Analyzing,

        [EnumMember(Value = "analyzing_failed")]
        AnalyzingFailed,

        [EnumMember(Value = "waiting_for_confirmation")]
        WaitingForConfirmation,

        [EnumMember(Value = "importing")]
        Importing,

        [EnumMember(Value = "importing_failed")]
        ImportingFailed,

        [EnumMember(Value = "completed")]
        Completed,

        [EnumMember(Value = "confirmed")]
        Confirmed,
    }

    public class CreateImportUrlResponse : ApiResponse
    {
        public string UploadUrl { get; set; }
        public string Path { get; set; }
    }

    public class CreateImportResponse : ApiResponse
    {
        public ImportTask ImportTask { get; set; }
    }

    public class ImportHistoryItem
    {
        public DateTimeOffset CreatedAt { get; set; }
        public ImportState NextState { get; set; }
        public ImportState PrevState { get; set; }
    }

    public class ImportTask
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public ImportMode Mode { get; set; }
        public long Size { get; set; }
        public ImportState State { get; set; }
        public List<ImportHistoryItem> History { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class GetImportResponse : CreateImportResponse
    {
    }

    public class ListImportsOptions : IQueryParameterConvertible
    {
        public int Limit { get; set; }
        public int Offset { get; set; }

        public List<KeyValuePair<string, string>> ToQueryParameters()
        {
            var parameters = new List<KeyValuePair<string, string>>(2);

            if (Limit > 0)
            {
                parameters.Add(new KeyValuePair<string, string>("limit", Limit.ToString(CultureInfo.InvariantCulture)));
            }

            if (Offset > 0)
            {
                parameters.Add(new KeyValuePair<string, string>("offset", Offset.ToString(CultureInfo.InvariantCulture)));
            }

            return parameters;
        }
    }

    public class ListImportsResponse : ApiResponse
    {
        public List<ImportTask> ImportTasks { get; set; }
    }
}