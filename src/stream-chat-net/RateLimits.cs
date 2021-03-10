using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using StreamChat.Rest;

namespace StreamChat
{
    public class RateLimitsInfo
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "limit")]
        public int Limit { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "remaining")]
        public int Remaining { get; internal set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reset")]
        public int Reset { get; internal set; }
    }

    public class RateLimitsMap
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "server_side")]
        public Dictionary<string, RateLimitsInfo> ServerSide { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "android")]
        public Dictionary<string, RateLimitsInfo> Android { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ios")]
        public Dictionary<string, RateLimitsInfo> IOS { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "web")]
        public Dictionary<string, RateLimitsInfo> Web { get; internal set; }
    }

    public class GetRateLimitsOptions
    {
        bool _serverSide = false;
        bool _android = false;
        bool _ios = false;
        bool _web = false;
        List<string> _endpoints = new List<string>();

        public QueryChannelsOptions WithServerSide()
        {
            _serverSide = true;
            return this;
        }

        public QueryChannelsOptions WithAndroid()
        {
            _android = true;
            return this;
        }

        public QueryChannelsOptions WithIOS()
        {
            _ios = true;
            return this;
        }

        public QueryChannelsOptions WithWeb()
        {
            _web = true;
            return this;
        }

        public QueryChannelsOptions WithEndpoint(string endpoint)
        {
            _endpoints.add(endpoint);
            return this;
        }

        public QueryChannelsOptions WithEndpoints(List<string> endpoints)
        {
            _endpoints = endpoints;
            return this;
        }

        internal void Apply(RestRequest request)
        {
            if (_serverSide) {
                request.AddQueryParameter("server_side", "true");
            }
            if (_android) {
                request.AddQueryParameter("android", "true");
            }
            if (_ios) {
                request.AddQueryParameter("ios", "true");
            }
            if (_web) {
                request.AddQueryParameter("web", "true");
            }
            if (_endpoints.Count > 0) {
                request.AddQueryParameter("endpoints", string.Join(".", _endpoints.ToArray()));
            }
        }
    }
}
