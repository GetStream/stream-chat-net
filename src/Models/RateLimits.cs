using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace StreamChat.Models
{
    public class RateLimitsInfo
    {
        /// <summary>The total limit allowed for the resource requested (i.e. 5000).</summary>
        public int Limit { get; set; }

        /// <summary>The remaining limit (i.e. 4999).</summary>
        public int Remaining { get; set; }

        /// <summary>When the current limit will reset.</summary>
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset Reset { get; set; }
    }

    public class RateLimitsMap : ApiResponse
    {
        public Dictionary<string, RateLimitsInfo> ServerSide { get; set; }
        public Dictionary<string, RateLimitsInfo> Android { get; set; }
        public Dictionary<string, RateLimitsInfo> IOS { get; set; }
        public Dictionary<string, RateLimitsInfo> Web { get; set; }
    }

    public class GetRateLimitsOptions : IQueryParameterConvertible
    {
        private bool _serverSide;
        private bool _android;
        private bool _ios;
        private bool _web;
        private List<string> _endpoints = new List<string>();

        public GetRateLimitsOptions WithServerSide()
        {
            _serverSide = true;
            return this;
        }

        public GetRateLimitsOptions WithAndroid()
        {
            _android = true;
            return this;
        }

        public GetRateLimitsOptions WithIOS()
        {
            _ios = true;
            return this;
        }

        public GetRateLimitsOptions WithWeb()
        {
            _web = true;
            return this;
        }

        public GetRateLimitsOptions WithEndpoint(string endpoint)
        {
            _endpoints.Add(endpoint);
            return this;
        }

        public GetRateLimitsOptions WithEndpoints(List<string> endpoints)
        {
            _endpoints = endpoints;
            return this;
        }

        public List<KeyValuePair<string, string>> ToQueryParameters()
        {
            var parameters = new List<KeyValuePair<string, string>>();

            if (_serverSide)
                parameters.Add(new KeyValuePair<string, string>("server_side", "true"));

            if (_android)
                parameters.Add(new KeyValuePair<string, string>("android", "true"));

            if (_ios)
                parameters.Add(new KeyValuePair<string, string>("ios", "true"));

            if (_web)
                parameters.Add(new KeyValuePair<string, string>("web", "true"));

            if (_endpoints.Count > 0)
                parameters.Add(new KeyValuePair<string, string>("endpoints", string.Join(",", _endpoints)));

            return parameters;
        }
    }
}
