using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
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

        private readonly List<ChannelWithConfig> _testChannels = new List<ChannelWithConfig>();
        private readonly List<string> _testChannelTypes = new List<string>();

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            const int chunkSize = 50;

            var cids = _testChannels.Select(x => x.Cid).ToArray();
            for (var i = 0; i < cids.Length; i += chunkSize)
            {
                var chunk = cids.Skip(i).Take(chunkSize).ToArray();

                try
                {
                    var resp = await _channelClient.DeleteChannelsAsync(chunk, hardDelete: true);
                    await WaitUntilTaskSucceedsAsync(resp.TaskId);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception thrown while deleting channels: {e.Message}. Channels to delete: {string.Join(", ", chunk)}");
                }
            }

            _testChannels.Clear();

            foreach (var channelType in _testChannelTypes)
            {
                try
                {
                    await _channelTypeClient.DeleteChannelTypeAsync(channelType);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception thrown while deleting channel type: {e.Message}. Channel type to delete: {channelType}");
                }
            }

            _testChannelTypes.Clear();
        }

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

        protected async Task<ChannelWithConfig> CreateChannelAsync(string createdByUserId, IEnumerable<string> members = null, bool autoDelete = true)
        {
            var channelResp = await _channelClient.GetOrCreateAsync("messaging", Guid.NewGuid().ToString(), new ChannelGetRequest
            {
                Data = new ChannelRequest
                {
                    Members = members?.Select((x, idx) => new ChannelMember { UserId = x, Role = idx == 0 ? Role.Admin : null }),
                    CreatedBy = new UserRequest { Id = createdByUserId },
                },
            });

            if (autoDelete)
            {
                _testChannels.Add(channelResp.Channel);
            }

            return channelResp.Channel;
        }

        protected async Task TryDeleteChannelAsync(string cid)
        {
            var resp = await _channelClient.DeleteChannelsAsync(new[] { cid }, hardDelete: true);
            await WaitUntilTaskSucceedsAsync(resp.TaskId);
        }

        protected async Task<ChannelTypeWithStringCommandsResponse> CreateChannelTypeAsync(string name = null, bool autoDelete = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = Guid.NewGuid().ToString();
            }

            var channelType = await _channelTypeClient.CreateChannelTypeAsync(
                new ChannelTypeWithStringCommandsRequest
                {
                    Name = name,
                });

            if (autoDelete)
            {
                _testChannelTypes.Add(channelType.Name);
            }

            return channelType;
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

        /// <summary>
        /// Execute test with multiple attempts. If the test throws an exception, it will be repeated few more times before it fails.
        /// </summary>
        /// <param name="testBody">Put the whole body of the test. </param>
        /// <param name="attempts">How many times to try</param>
        /// <param name="attemptTimeoutMs">delay between a failed try</param>
        /// <exception cref="ArgumentException">Throws ArgumentException If max attempts or timeout exceeds the limit</exception>
        protected async Task TryMultipleAsync(Func<Task> testBody,
            int attempts = 5, int attemptTimeoutMs = 500)
        {
            const int maxAttempts = 20;
            const int maxAttemptTimeout = 1500;

            if (attempts > maxAttempts)
            {
                throw new ArgumentException($"Max attempts: {maxAttempts}, given: {attempts}");
            }

            if (attemptTimeoutMs > maxAttemptTimeout)
            {
                throw new ArgumentException($"Max attempt timeout: {maxAttemptTimeout}, given: {attemptTimeoutMs}");
            }

            for (int i = 0; i < attempts; i++)
            {
                try
                {
                    await testBody();
                }
                catch (Exception)
                {
                    if (i == attempts - 1)
                    {
                        throw;
                    }

                    await Task.Delay(attemptTimeoutMs);
                    continue;
                }

                return;
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
