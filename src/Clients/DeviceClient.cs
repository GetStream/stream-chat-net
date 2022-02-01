using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public class DeviceClient : ClientBase, IDeviceClient
    {
        internal DeviceClient(IRestClient client) : base(client)
        {
        }

        public async Task<ApiResponse> AddDeviceAsync(Device device)
            => await ExecuteRequestAsync<ApiResponse>("devices",
                HttpMethod.POST,
                HttpStatusCode.Created,
                device);

        public async Task<GetDevicesResponse> GetDevicesAsync(string userId)
            => await ExecuteRequestAsync<GetDevicesResponse>("devices",
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user_id", userId),
                });

        public async Task<ApiResponse> RemoveDeviceAsync(string deviceId, string userId)
            => await ExecuteRequestAsync<ApiResponse>("devices",
                HttpMethod.DELETE,
                HttpStatusCode.OK,
                queryParams: new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user_id", userId),
                    new KeyValuePair<string, string>("id", deviceId),
                });
    }
}