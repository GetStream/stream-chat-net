using System;
using System.Collections.Generic;

namespace StreamChat.Models
{
    public class Reaction : CustomDataBase
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public UserRequest User { get; set; }
        public string Type { get; set; }
        public int? Score { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }

    public class ReactionRequest : CustomDataBase
    {
        public string MessageId { get; set; }

        /// <summary>
        /// Id of a user who reacted to a message.
        /// </summary>
        public string UserId { get; set; }
        public string Type { get; set; }
        public int? Score { get; set; }
    }

    public class ReactionSendRequest
    {
        public ReactionRequest Reaction { get; set; }
        public bool? SkipPush { get; set; }
        public bool? EnforceUnique { get; set; }
    }

    public class ReactionResponse : ApiResponse
    {
        public Message Message { get; set; }
        public Reaction Reaction { get; set; }
    }

    public class GetReactionsResponse : ApiResponse
    {
        public List<Reaction> Reactions { get; set; }
    }
}
