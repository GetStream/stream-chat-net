using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StreamChat.Clients;
using StreamChat.Models;

namespace StreamChatTests
{
    public abstract class TestBase
    {
        protected static readonly IAppClient _appClient = TestClientFactory.GetAppClient();
        protected static readonly IBlocklistClient _blocklistClient = TestClientFactory.GetBlocklistClient();
        protected static readonly IChannelClient _channelClient = TestClientFactory.GetChannelClient();
        protected static readonly IChannelTypeClient _channelTypeClient = TestClientFactory.GetChannelTypeClient();
        protected static readonly ICommandClient _commandClient = TestClientFactory.GetCommandClient();
        protected static readonly IDeviceClient _deviceClient = TestClientFactory.GetDeviceClient();
        protected static readonly IEventClient _eventClient = TestClientFactory.GetEventClient();
        protected static readonly IFlagClient _flagClient = TestClientFactory.GetFlagClient();
        protected static readonly IImportClient _importClient = TestClientFactory.GetImportClient();
        protected static readonly IMessageClient _messageClient = TestClientFactory.GetMessageClient();
        protected static readonly IPermissionClient _permissionClient = TestClientFactory.GetPermissionClient();
        protected static readonly IReactionClient _reactionClient = TestClientFactory.GetReactionClient();
        protected static readonly IUserClient _userClient = TestClientFactory.GetUserClient();
        protected static readonly ITaskClient _taskClient = TestClientFactory.GetTaskClient();

        protected async Task WaitForAsync(Func<Task<bool>> condition, int timeout = 5000, int delay = 500)
        {
            var start = DateTimeOffset.UtcNow;

            while (true)
            {
                if (DateTimeOffset.UtcNow - start > TimeSpan.FromMilliseconds(timeout))
                {
                    throw new TimeoutException();
                }

                try
                {
                    var result = await condition();
                    if (result)
                        break;
                }
                catch
                {
                }

                await Task.Delay(delay);
            }
        }

        protected async Task<UserRequest> UpsertNewUserAsync()
        {
            var user = new UserRequest
            {
                Id = Guid.NewGuid().ToString(),
                Role = Role.Admin,
            };

            await _userClient.UpsertAsync(user);

            return user;
        }

        protected async Task<ChannelWithConfig> CreateChannelAsync(string createdByUserId, IEnumerable<string> members)
        {
            var channelResp = await _channelClient.GetOrCreateAsync("messaging", Guid.NewGuid().ToString(), new ChannelGetRequest
            {
                Data = new ChannelRequest
                {
                    Members = members.Select((x, idx) => new ChannelMember { UserId = x, Role = idx == 0 ? Role.Admin : null }),
                    CreatedBy = new UserRequest { Id = createdByUserId },
                },
            });

            return channelResp.Channel;
        }

        protected async Task TryDeleteChannelAsync(string cid)
        {
            var resp = await _channelClient.DeleteChannelsAsync(new[] { cid }, hardDelete: true);
            await WaitUntilTaskSucceedsAsync(resp.TaskId);
        }

        protected async Task TryDeleteUsersAsync(params string[] userIds)
        {
            try
            {
                await _userClient.DeleteManyAsync(
                                new DeleteUsersRequest()
                                .WithUserIds(userIds)
                                .WithUserDeletionStrategy(DeletionStrategy.Hard)
                                .WithMessagesDeletionStrategy(DeletionStrategy.Hard)
                                .WithConversationsDeletionStrategy(DeletionStrategy.Hard));
            }
            catch
            {
            }
        }

        private async Task WaitUntilTaskSucceedsAsync(string taskId)
        {
            try
            {
                await WaitForAsync(async () =>
                {
                    var status = await _taskClient.GetTaskStatusAsync(taskId);

                    return status.Status == AsyncTaskStatus.Completed;
                }, timeout: 10000, delay: 1000);
            }
            catch (TimeoutException)
            {
            }
        }
    }
}
