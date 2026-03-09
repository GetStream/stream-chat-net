using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    /// <summary>
    /// Represents a metric value for a specific date.
    /// </summary>
    public class DailyValue
    {
        /// <summary>Date in YYYY-MM-DD format.</summary>
        [JsonProperty("date")]
        public string Date { get; set; }

        /// <summary>Metric value for this date.</summary>
        [JsonProperty("value")]
        public long Value { get; set; }
    }

    /// <summary>
    /// Statistics for a single metric with optional daily breakdown.
    /// </summary>
    public class MetricStats
    {
        /// <summary>Per-day values (only present in daily mode).</summary>
        [JsonProperty("daily")]
        public List<DailyValue> Daily { get; set; }

        /// <summary>Aggregated total value.</summary>
        [JsonProperty("total")]
        public long Total { get; set; }
    }

    /// <summary>
    /// Team-level usage statistics for multi-tenant apps.
    /// </summary>
    public class TeamUsageStats
    {
        /// <summary>Team identifier (empty string for users not assigned to any team).</summary>
        [JsonProperty("team")]
        public string Team { get; set; }

        // Daily activity metrics (total = SUM of daily values)

        /// <summary>Daily active users.</summary>
        [JsonProperty("users_daily")]
        public MetricStats UsersDaily { get; set; }

        /// <summary>Daily messages sent.</summary>
        [JsonProperty("messages_daily")]
        public MetricStats MessagesDaily { get; set; }

        /// <summary>Daily translations.</summary>
        [JsonProperty("translations_daily")]
        public MetricStats TranslationsDaily { get; set; }

        /// <summary>Daily image moderations.</summary>
        [JsonProperty("image_moderations_daily")]
        public MetricStats ImageModerationsDaily { get; set; }

        // Peak metrics (total = MAX of daily values)

        /// <summary>Peak concurrent users.</summary>
        [JsonProperty("concurrent_users")]
        public MetricStats ConcurrentUsers { get; set; }

        /// <summary>Peak concurrent connections.</summary>
        [JsonProperty("concurrent_connections")]
        public MetricStats ConcurrentConnections { get; set; }

        // Rolling/cumulative metrics (total = LATEST daily value)

        /// <summary>Total users.</summary>
        [JsonProperty("users_total")]
        public MetricStats UsersTotal { get; set; }

        /// <summary>Users active in last 24 hours.</summary>
        [JsonProperty("users_last_24_hours")]
        public MetricStats UsersLast24Hours { get; set; }

        /// <summary>MAU - users active in last 30 days.</summary>
        [JsonProperty("users_last_30_days")]
        public MetricStats UsersLast30Days { get; set; }

        /// <summary>Users active this month.</summary>
        [JsonProperty("users_month_to_date")]
        public MetricStats UsersMonthToDate { get; set; }

        /// <summary>Engaged MAU.</summary>
        [JsonProperty("users_engaged_last_30_days")]
        public MetricStats UsersEngagedLast30Days { get; set; }

        /// <summary>Engaged users this month.</summary>
        [JsonProperty("users_engaged_month_to_date")]
        public MetricStats UsersEngagedMonthToDate { get; set; }

        /// <summary>Total messages.</summary>
        [JsonProperty("messages_total")]
        public MetricStats MessagesTotal { get; set; }

        /// <summary>Messages in last 24 hours.</summary>
        [JsonProperty("messages_last_24_hours")]
        public MetricStats MessagesLast24Hours { get; set; }

        /// <summary>Messages in last 30 days.</summary>
        [JsonProperty("messages_last_30_days")]
        public MetricStats MessagesLast30Days { get; set; }

        /// <summary>Messages this month.</summary>
        [JsonProperty("messages_month_to_date")]
        public MetricStats MessagesMonthToDate { get; set; }
    }

    /// <summary>
    /// Options for querying team usage stats.
    /// </summary>
    public class QueryTeamUsageStatsOptions
    {
        /// <summary>
        /// Month in YYYY-MM format (e.g., '2026-01'). Mutually exclusive with start_date/end_date.
        /// Returns aggregated monthly values.
        /// </summary>
        [JsonProperty("month")]
        public string Month { get; set; }

        /// <summary>
        /// Start date in YYYY-MM-DD format. Used with end_date for custom date range. Returns daily breakdown.
        /// </summary>
        [JsonProperty("start_date")]
        public string StartDate { get; set; }

        /// <summary>
        /// End date in YYYY-MM-DD format. Used with start_date for custom date range. Returns daily breakdown.
        /// </summary>
        [JsonProperty("end_date")]
        public string EndDate { get; set; }

        /// <summary>Maximum number of teams to return per page (default: 30, max: 30).</summary>
        [JsonProperty("limit")]
        public int? Limit { get; set; }

        /// <summary>Cursor for pagination to fetch next page of teams.</summary>
        [JsonProperty("next")]
        public string Next { get; set; }
    }

    /// <summary>
    /// Response from querying team usage stats.
    /// </summary>
    public class QueryTeamUsageStatsResponse : ApiResponse
    {
        /// <summary>Array of team usage statistics.</summary>
        [JsonProperty("teams")]
        public List<TeamUsageStats> Teams { get; set; }

        /// <summary>Cursor for pagination to fetch next page.</summary>
        [JsonProperty("next")]
        public string Next { get; set; }
    }
}
