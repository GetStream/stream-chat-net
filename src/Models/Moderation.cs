using System;
using System.Collections.Generic;
using StreamChat.Utils;

namespace StreamChat.Models
{
    public class BanRequest
    {
        /// <summary>User ID who issued a ban</summary>
        public string BannedById { get; set; }

        /// <summary>ID of user to ban</summary>
        public string TargetUserId { get; set; }

        /// <summary>User ID which server acts upon</summary>
        public string UserId { get; set; }

        /// <summary>Ban reason</summary>
        public string Reason { get; set; }

        /// <summary>Whether to perform IP ban or not</summary>
        public bool? IpBan { get; set; }

        /// <summary>Timeout of ban in minutes. User will be unbanned after this period of time.</summary>
        public int? Timeout { get; set; }

        /// <summary>Whether to perform shadow ban or not</summary>
        public bool? Shadow { get; set; }

        /// <summary>Channel type to ban user in</summary>
        public string Type { get; set; }

        /// <summary>Channel ID to ban user in</summary>
        public string Id { get; set; }

        /// <summary>When true, the user will be automatically banned from all future channels created by the user who issued the ban</summary>
        public bool? BanFromFutureChannels { get; set; }
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
        public User BannedBy { get; set; }
    }

    public class FutureChannelBan
    {
        public User User { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public string Reason { get; set; }
        public bool Shadow { get; set; }
    }

    public class QueryFutureChannelBansRequest : IQueryParameterConvertible
    {
        public string UserId { get; set; }
        public bool? ExcludeExpiredBans { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }

        public List<KeyValuePair<string, string>> ToQueryParameters()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("payload", StreamJsonConverter.SerializeObject(this)),
            };
        }
    }

    public class QueryFutureChannelBansResponse : ApiResponse
    {
        public List<FutureChannelBan> Bans { get; set; }
    }

    public class QueryBannedUsersRequest : IQueryParameterConvertible
    {
        public Dictionary<string, object> FilterConditions { get; set; }
        public IEnumerable<SortParameter> Sorts { get; set; }
        public DateTimeOffset? CreatedAtAfterOrEqual { get; set; }
        public DateTimeOffset? CreatedAtAfter { get; set; }
        public DateTimeOffset? CreatedAtBeforeOrEqual { get; set; }
        public DateTimeOffset? CreatedAtBefore { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public string UserId { get; set; }
        public UserRequest User { get; set; }

        public List<KeyValuePair<string, string>> ToQueryParameters()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("payload", StreamJsonConverter.SerializeObject(this)),
            };
        }
    }

    public class QueryFlagReportsRequest
    {
        private const int DefaultOffset = 0;
        private const int DefaultLimit = 20;

        public Dictionary<string, object> FilterConditions { get; set; }
        public int Limit { get; set; } = DefaultLimit;
        public int Offset { get; set; } = DefaultOffset;
    }

    public class FlagReport
    {
        public string Id { get; set; }
        public Message Message { get; set; }
        public int FlagsCount { get; set; }
        public string MessageUserId { get; set; }
        public string ChannelCid { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class QueryFlagReportsResponse : ApiResponse
    {
        public List<FlagReport> FlagReports { get; set; }
    }

    public class ReviewFlagReportRequest
    {
        public string ReviewResult { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, object> ReviewDetails { get; set; }
    }

    public class ExtendedFlagReport : FlagReport
    {
        public string ReviewResult { get; set; }
        public Dictionary<string, object> ReviewDetails { get; set; }
        public DateTimeOffset ReviewedAt { get; set; }
        public User ReviewedBy { get; set; }
    }

    public class ReviewFlagReportResponse : ApiResponse
    {
        public ExtendedFlagReport FlagReport { get; set; }
    }
}