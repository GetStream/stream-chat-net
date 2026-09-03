using System.Collections.Generic;
using System.Net;
using System.Threading;
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

        public async Task<QueryChannelResponse> QueryChannelsAsync(QueryChannelsOptions opts, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<QueryChannelResponse>("channels",
                HttpMethod.POST,
                HttpStatusCode.Created,
                opts,
                cancellationToken: cancellationToken);

        public async Task<AsyncOperationResponse> ExportChannelAsync(ExportChannelItem request, CancellationToken cancellationToken = default)
            => await ExportChannelsAsync(
                new ExportChannelRequest { Channels = new[] { request } },
                cancellationToken: cancellationToken);

        public async Task<AsyncOperationResponse> ExportChannelsAsync(ExportChannelRequest request, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<AsyncOperationResponse>("export_channels",
                HttpMethod.POST,
                HttpStatusCode.Created,
                request,
                cancellationToken: cancellationToken);

        public async Task<ExportChannelsStatusResponse> GetExportChannelsStatusAsync(string taskId, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<ExportChannelsStatusResponse>($"export_channels/{taskId}",
                HttpMethod.GET,
                HttpStatusCode.OK,
                cancellationToken: cancellationToken);

        public async Task<AsyncOperationResponse> DeleteChannelsAsync(IEnumerable<string> cids, bool hardDelete = false, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<AsyncOperationResponse>("channels/delete",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new { cids = cids, hard_delete = hardDelete },
                cancellationToken: cancellationToken);

        public async Task<AsyncOperationResponse> UpdateChannelsBatchAsync(ChannelsBatchOptions options, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<AsyncOperationResponse>("channels/batch",
                HttpMethod.PUT,
                HttpStatusCode.Created,
                options,
                cancellationToken: cancellationToken);

        public ChannelBatchUpdater BatchUpdater() => new ChannelBatchUpdater(this);

        public async Task<ApiResponse> HideAsync(string channelType, string channelId, string userId, bool clearHistory = false, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}/hide",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new
                {
                    user_id = userId,
                    clear_history = clearHistory,
                },
                cancellationToken: cancellationToken);

        public async Task<ApiResponse> ShowAsync(string channelType, string channelId, string userId, CancellationToken cancellationToken = default)
            => await ExecuteRequestAsync<ApiResponse>($"channels/{channelType}/{channelId}/show",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new { user_id = userId },
                cancellationToken: cancellationToken);
    }
}
