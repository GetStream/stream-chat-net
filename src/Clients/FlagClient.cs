using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class FlagClient : ClientBase, IFlagClient
    {
        internal FlagClient(IRestClient client) : base(client)
        {
        }

        public async Task<ApiResponse> FlagUserAsync(string flaggedId, string flaggerId)
            => await PostFlagAsync(flaggedId, flaggerId, "flag", "user");

        public async Task<ApiResponse> FlagMessageAsync(string flaggedId, string flaggerId)
            => await PostFlagAsync(flaggedId, flaggerId, "flag", "message");

        private async Task<ApiResponse> PostFlagAsync(string dest, string src, string op, string kind)
        {
            var req = new FlagCreateRequest { UserId = src };

            if (kind.Equals("user"))
                req.TargetUserId = dest;
            else
                req.TargetMessageId = dest;

            return await ExecuteRequestAsync<ApiResponse>($"moderation/{op}", HttpMethod.POST, HttpStatusCode.Created, req);
        }

        public async Task<QueryMessageFlagsResponse> QueryMessageFlags(FlagMessageQueryRequest request)
            => await ExecuteRequestAsync<QueryMessageFlagsResponse>("moderation/flags/message",
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: request.ToQueryParameters());
    }
}