using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class BanRequest
    {
        /// <summary>User ID who issued a ban</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "banned_by_id")]
        public string BannedById { get; set; }

        /// <summary>ID of user to ban</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "target_user_id")]
        public string TargetUserId { get; set; }

        /// <summary>User ID which server acts upon</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        /// <summary>Ban reason</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reason")]
        public string Reason { get; set; }

        /// <summary>Whether to perform IP ban or not</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ip_ban")]
        public bool? IpBan { get; set; }

        /// <summary>Timeout of ban in minutes. User will be unbanned after this period of time.</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "timeout")]
        public int? Timeout { get; set; }

        /// <summary>Whether to perform shadow ban or not</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "shadow")]
        public bool? Shadow { get; set; }

        /// <summary>Channel type to ban user in</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>Channel ID to ban user in</summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string Id { get; set; }
    }

    public class ShadowBanRequest : BanRequest
    {
        public BanRequest ToBanRequest()
        {
            Shadow = true;
            return this;
        }
    }

    public class Ban
    {
        public Channel Channel { get; set; }
        public User User { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public bool Shadow { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "banned_by")]
        public User BannedBy { get; set; }
    }

    public class QueryBannedUsersRequest : IQueryParameterConvertible
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "filter_conditions")]
        public Dictionary<string, object> FilterConditions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "sort")]
        public IEnumerable<SortParameter> Sorts { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at_after_or_equal")]
        public DateTimeOffset? CreatedAtAfterOrEqual { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at_after")]
        public DateTimeOffset? CreatedAtAfter { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at_before_or_equal")]
        public DateTimeOffset? CreatedAtBeforeOrEqual { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at_before")]
        public DateTimeOffset? CreatedAtBefore { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "limit")]
        public int? Limit { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "offset")]
        public int? Offset { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public UserRequest User { get; set; }

        public List<KeyValuePair<string, string>> ToQueryParameters()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("payload", JsonConvert.SerializeObject(this)),
            };
        }
    }

    public class QueryFlagReportsRequest
    {
        private const int DefaultOffset = 0;
        private const int DefaultLimit = 20;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "filter_conditions")]
        public Dictionary<string, object> FilterConditions { get; set; }
        public int Limit { get; set; } = DefaultLimit;
        public int Offset { get; set; } = DefaultOffset;
    }

    public class FlagReport
    {
        public string Id { get; set; }
        public Message Message { get; set; }

        [JsonProperty("flags_count")]
        public int FlagsCount { get; set; }

        [JsonProperty("message_user_id")]
        public string MessageUserId { get; set; }

        [JsonProperty("channel_cid")]
        public string ChannelCid { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class QueryFlagReportsResponse : ApiResponse
    {
        [JsonProperty("flag_reports")]
        public List<FlagReport> FlagReports { get; set; }
    }

    public class ReviewFlagReportRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "review_result")]
        public string ReviewResult { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "review_details")]
        public Dictionary<string, object> ReviewDetails { get; set; }
    }

    public class ExtendedFlagReport : FlagReport
    {
        [JsonProperty("review_result")]
        public string ReviewResult { get; set; }

        [JsonProperty("review_details")]
        public Dictionary<string, object> ReviewDetails { get; set; }

        [JsonProperty("reviewed_at")]
        public DateTimeOffset ReviewedAt { get; set; }

        [JsonProperty("reviewed_by")]
        public User ReviewedBy { get; set; }
    }

    public class ReviewFlagReportResponse : ApiResponse
    {
        [JsonProperty("flag_report")]
        public ExtendedFlagReport FlagReport { get; set; }
    }
}