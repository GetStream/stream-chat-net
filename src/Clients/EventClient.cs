using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class EventClient : ClientBase, IEventClient
    {
        internal EventClient(IRestClient client) : base(client)
        {
        }

        public async Task<SendEventResponse> SendEventAsync(string channelType, string channelId, Event @event)
            => await ExecuteRequestAsync<SendEventResponse>($"channels/{channelType}/{channelId}/event",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new { @event = @event });
    }
}