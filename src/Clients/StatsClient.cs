using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class StatsClient : ClientBase, IStatsClient
    {
        internal StatsClient(IRestClient client) : base(client)
        {
        }

        public async Task<QueryTeamUsageStatsResponse> QueryTeamUsageStatsAsync(QueryTeamUsageStatsOptions options = null)
            => await ExecuteRequestAsync<QueryTeamUsageStatsResponse>("stats/team_usage",
                HttpMethod.POST,
                HttpStatusCode.Created,
                options ?? new QueryTeamUsageStatsOptions());
    }
}
