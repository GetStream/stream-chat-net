using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class ReactionClient : ClientBase, IReactionClient
    {
        internal ReactionClient(IRestClient client) : base(client)
        {
        }

        public async Task<ReactionResponse> SendReactionAsync(string messageId, string reactionType, string userId)
            => await SendReactionAsync(messageId, new ReactionRequest { Type = reactionType, UserId = userId });

        public async Task<ReactionResponse> SendReactionAsync(string messageId, string reactionType, string userId, bool skipPush)
            => await SendReactionAsync(messageId, new ReactionRequest { Type = reactionType, UserId = userId }, skipPush);

        public async Task<ReactionResponse> SendReactionAsync(string messageId, ReactionRequest reaction, bool skipPush = false)
            => await ExecuteRequestAsync<ReactionResponse>($"messages/{messageId}/reaction",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new ReactionSendRequest { Reaction = reaction, SkipPush = skipPush });

        public async Task<ReactionResponse> DeleteReactionAsync(string messageId, string reactionType, string userId)
            => await ExecuteRequestAsync<ReactionResponse>($"messages/{messageId}/reaction/{reactionType}",
                HttpMethod.DELETE,
                HttpStatusCode.OK,
                queryParams: new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user_id", userId),
                });

        public async Task<GetReactionsResponse> GetReactionsAsync(string messageId, int offset = 0, int limit = 50)
            => await ExecuteRequestAsync<GetReactionsResponse>($"messages/{messageId}/reactions",
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("offset", offset.ToString()),
                    new KeyValuePair<string, string>("limit", limit.ToString()),
                });
    }
}