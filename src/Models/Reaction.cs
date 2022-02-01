using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class Reaction : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message_id")]
        public string MessageID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public UserRequest User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "score")]
        public int? Score { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class ReactionRequest : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message_id")]
        public string MessageID { get; set; }

        /// <summary>
        /// Id of a user who reacted to a message
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "score")]
        public int? Score { get; set; }
    }

    public class ReactionSendRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reaction")]
        public ReactionRequest Reaction { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "skip_push")]
        public bool? SkipPush { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "enforce_unique")]
        public bool? EnforceUnique { get; set; }
    }

    public class ReactionResponse : ApiResponse
    {
        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("reaction")]
        public Reaction Reaction { get; set; }
    }

    public class GetReactionsResponse : ApiResponse
    {
        [JsonProperty("reactions")]
        public List<Reaction> Reactions { get; set; }
    }
}
