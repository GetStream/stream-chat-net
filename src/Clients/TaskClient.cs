using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class TaskClient : ClientBase, ITaskClient
    {
        internal TaskClient(IRestClient client) : base(client)
        {
        }

        public async Task<AsyncTaskStatusResponse> GetTaskStatusAsync(string taskId)
            => await ExecuteRequestAsync<AsyncTaskStatusResponse>($"tasks/{taskId}",
                HttpMethod.GET,
                HttpStatusCode.OK);
    }
}