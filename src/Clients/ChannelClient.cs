using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;

namespace StreamChat.Clients
{
    public partial class ChannelClient : ClientBase, IChannelClient
    {
        internal ChannelClient(IRestClient client) : base(client)
        {
        }

        public async Task<QueryChannelResponse> QueryChannelsAsync(QueryChannelsOptions opts)
            => await ExecuteRequestAsync<QueryChannelResponse>("channels",
                HttpMethod.POST,
                HttpStatusCode.Created,
                opts);

        public async Task<AsyncOperationResponse> ExportChannelAsync(ExportChannelItem request)
            => await ExportChannelsAsync(new ExportChannelRequest { Channels = new[] { request } });

        public async Task<AsyncOperationResponse> ExportChannelsAsync(ExportChannelRequest request)
            => await ExecuteRequestAsync<AsyncOperationResponse>("export_channels",
                HttpMethod.POST,
                HttpStatusCode.Created,
                request);

        public async Task<ExportChannelsStatusResponse> GetExportChannelsStatusAsync(string taskId)
            => await ExecuteRequestAsync<ExportChannelsStatusResponse>($"export_channels/{taskId}",
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<AsyncOperationResponse> DeleteChannelsAsync(IEnumerable<string> cids, bool hardDelete = false)
            => await ExecuteRequestAsync<AsyncOperationResponse>("channels/delete",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new { cids = cids, hard_delete = hardDelete });

        public async Task<AsyncOperationResponse> UpdateChannelsBatchAsync(ChannelsBatchOptions options)
            => await ExecuteRequestAsync<AsyncOperationResponse>("channels/batch",
                HttpMethod.PUT,
                HttpStatusCode.Created,
                options);

        public ChannelBatchUpdater BatchUpdater() => new ChannelBatchUpdater(this);

        public async Task<ApiResponse> HideAsync(string channelType, string channelId, string userId, bool clearHistory = false)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}/hide",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new
                {
                    user_id = userId,
                    clear_history = clearHistory,
                });

        public async Task<ApiResponse> ShowAsync(string channelType, string channelId, string userId)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}/show",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new { user_id = userId });
    }
}
