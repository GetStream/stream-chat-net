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
}