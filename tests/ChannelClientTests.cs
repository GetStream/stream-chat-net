using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="ChannelClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class ChannelClientTests : TestBase
    {
        private UserRequest _user1;
        private UserRequest _user2;
        private UserRequest _user3;
        private ChannelWithConfig _channel;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            (_user1, _user2, _user3) = (await UpsertNewUserAsync(), await UpsertNewUserAsync(), await UpsertNewUserAsync());
            _channel = await CreateChannelAsync(createdByUserId: _user1.Id, members: new[] { _user1.Id, _user2.Id, _user3.Id });
            await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user1.Id, "text");
        }

        [OneTimeTearDown]
        public async Task TeardownAsync()
        {
            await TryDeleteChannelAsync(_channel.Cid);
            await TryDeleteUsersAsync(_user1.Id, _user2.Id, _user3.Id);
        }

        [Test]
        public async Task TestChannelCreateWithoutIdAsync()
        {
            var chanData = new ChannelRequest();
            chanData.SetData("name", "one big party");
            chanData.SetData("food", new[] { "pizza", "gabagool" });
            chanData.CreatedBy = new UserRequest { Id = _user1.Id };
            chanData.Members = new List<ChannelMember>
            {
                new ChannelMember { UserId = _user1.Id },
                new ChannelMember { UserId = _user2.Id },
            };

            var chanState = await _channelClient.GetOrCreateAsync("messaging", new ChannelGetRequest
            {
                Data = chanData,
            });

            chanState.Channel.Id.Should().NotBeEmpty();
            chanState.Channel.MemberCount.Should().BeGreaterThan(0);
            chanState.Members.Count.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task TestChannelQueryAsync()
        {
            var currentChannelState = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, new ChannelGetRequest
            {
                State = true,
                Watch = false,
                MembersPagination = new PaginationParams
                {
                    Limit = 1,
                    Offset = 1,
                },
            });

            currentChannelState.Members.Count.Should().Be(1);
        }

        [Test]
        public async Task TestQueryMembersAsync()
        {
            var res = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = _channel.Type,
                Id = _channel.Id,
                UserId = _user1.Id,
                FilterConditions = new Dictionary<string, object>(),
                Limit = 1,
            });

            res.Members.Should().HaveCount(1);
        }

        [Test]
        public async Task TestChannelUpdateAsync()
        {
            var expectedChannel = new ChannelUpdateRequest { Data = new ChannelRequest() };
            expectedChannel.Data.SetData("plain", Guid.NewGuid().ToString());
            expectedChannel.Data.SetData("complex", new[] { Guid.NewGuid().ToString() });

            var actualChannel = await _channelClient.UpdateAsync(_channel.Type, _channel.Id, expectedChannel);

            actualChannel.Channel.GetData<string>("plain").Should().BeEquivalentTo(expectedChannel.Data.GetData<string>("plain"));
            actualChannel.Channel.GetData<string[]>("complex").Should().BeEquivalentTo(expectedChannel.Data.GetData<string[]>("complex"));
        }

        [Test]
        public async Task TestChannelPartialUpdateAsync()
        {
            var channelUpdates = new PartialUpdateChannelRequest
            {
                Set = new Dictionary<string, object> { { "color", "blue" }, { "age", 27 } },
            };

            var actualChannel = await _channelClient.PartialUpdateAsync(_channel.Type, _channel.Id, channelUpdates);

            actualChannel.Channel.GetData<string>("color").Should().BeEquivalentTo((string)channelUpdates.Set["color"]);
            actualChannel.Channel.GetData<int>("age").Should().Be((int)channelUpdates.Set["age"]);
        }

        [Test]
        public async Task TestDeleteChannelAsync()
        {
            // Let's not delete the channel that the other tests are using, so let's create a new one
            var channel = await CreateChannelAsync(createdByUserId: _user1.Id, members: new[] { _user1.Id, _user2.Id, _user3.Id });

            await WaitForAsync(async () =>
            {
                try
                {
                    await _channelClient.DeleteAsync(channel.Type, channel.Id);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });

            var resp = await _channelClient.QueryChannelsAsync(QueryChannelsOptions.Default.WithFilter(new Dictionary<string, object>
            {
                { "cid", new Dictionary<string, object> { { "$eq", channel.Cid } } },
            }));
            resp.Channels.Count.Should().Be(0);
        }

        [Test]
        public async Task TestChannelTruncateAsync()
        {
            await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user1.Id, "text");
            var originalChannel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, new ChannelGetRequest { State = true });
            originalChannel.Messages.Should().NotBeEmpty();

            await _channelClient.TruncateAsync(_channel.Type, _channel.Id, new TruncateOptions { UserId = _user2.Id });

            var afterTruncateChannel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, new ChannelGetRequest { State = true });
            afterTruncateChannel.Messages.Should().BeEmpty();

            // this isn't deployed to the test environment, so uncomment it a bit later:
            // afterTruncateChannel.Channel.TruncatedBy.Id.Should().Be(_user2.Id);
        }

        [Test]
        public async Task TestQueryChannelsAsync()
        {
            var queryResp = await _channelClient.QueryChannelsAsync(QueryChannelsOptions.Default.WithFilter(new Dictionary<string, object>
            {
                { "cid", new Dictionary<string, object> { { "$eq", _channel.Cid } } },
            }));

            var read = queryResp.Channels[0].Reads[0];
            read.LastRead.Should().NotBe(default(DateTimeOffset));
            read.UnreadMessages.Should().NotBeNull();
            read.User.Should().NotBeNull();
        }

        [Test]
        public async Task TestHideAndShowChannelAsync()
        {
            Func<Task> hideCall = () => _channelClient.HideAsync(_channel.Type, _channel.Id, _user1.Id);
            Func<Task> showCall = () => _channelClient.ShowAsync(_channel.Type, _channel.Id, _user1.Id);

            await hideCall.Should().NotThrowAsync();
            await showCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestMuteUnmuteChannelAsync()
        {
            Func<Task> muteCall = () => _channelClient.MuteChannelAsync(new ChannelMuteRequest
            {
                ChannelCids = new[] { _channel.Cid },
                UserId = _user1.Id,
            });
            Func<Task> unmuteCall = () => _channelClient.UnmuteChannelAsync(new ChannelUnmuteRequest
            {
                ChannelCids = new[] { _channel.Cid },
                UserId = _user1.Id,
            });

            await muteCall.Should().NotThrowAsync();
            await unmuteCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestGetExportChannelAsync()
        {
            var resp = await _channelClient.ExportChannelAsync(new ExportChannelItem { Id = _channel.Id, Type = _channel.Type });

            await WaitForAsync(async () =>
            {
                var status = await _taskClient.GetTaskStatusAsync(resp.TaskId);

                return status.Status == AsyncTaskStatus.Completed;
            }, timeout: 10000);
        }

        [Test]
        public async Task TestAddMembersAsync()
        {
            var newUser = await UpsertNewUserAsync();

            await _channelClient.AddMembersAsync(_channel.Type, _channel.Id, newUser.Id);

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, ChannelGetRequest.WithoutWatching());
            channel.Members.Should().Contain(x => x.UserId == newUser.Id);
            await TryDeleteUsersAsync(new[] { newUser.Id });
        }

        [Test]
        public async Task TestRemoveMembersAsync()
        {
            await _channelClient.RemoveMembersAsync(_channel.Type, _channel.Id, new[] { _user3.Id });

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, ChannelGetRequest.WithoutWatching());
            channel.Members.Should().NotContain(x => x.UserId == _user3.Id);
        }

        [Test]
        public async Task TestAddModeratorsAsync()
        {
            await _channelClient.AddModeratorsAsync(_channel.Type, _channel.Id, new[] { _user2.Id });

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, ChannelGetRequest.WithoutWatching());
            var user2 = channel.Members.First(x => x.UserId == _user2.Id);
            user2.IsModerator.Should().BeTrue();
        }

        [Test]
        public async Task TestDemoteModeratorsAsync()
        {
            await _channelClient.AddModeratorsAsync(_channel.Type, _channel.Id, new[] { _user2.Id });

            await _channelClient.DemoteModeratorsAsync(_channel.Type, _channel.Id, new[] { _user2.Id });

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, ChannelGetRequest.WithoutWatching());
            var user2 = channel.Members.First(x => x.UserId == _user2.Id);
            user2.IsModerator.Should().BeNull();
        }

        [Test]
        public async Task TestAssignRoleAsync()
        {
            await _channelClient.AssignRolesAsync(_channel.Type, _channel.Id, new AssignRoleRequest
            {
                AssignRoles = new List<RoleAssignment>
                 {
                     new RoleAssignment
                     {
                         UserId = _user2.Id,
                         ChannelRole = Role.ChannelModerator,
                     },
                 },
            });

            var channel = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, ChannelGetRequest.WithoutWatching());
            var user2 = channel.Members.First(x => x.UserId == _user2.Id);
            user2.IsModerator.Should().BeTrue();
        }

        [Test]
        public async Task TestInvitationAndAcceptanceAsync()
        {
            var userToInvite = await UpsertNewUserAsync();
            Func<Task> inviteTask = () => _channelClient.InviteAsync(_channel.Type, _channel.Id, userToInvite.Id);
            Func<Task> acceptTask = () => _channelClient.AcceptInviteAsync(_channel.Type, _channel.Id, userToInvite.Id);

            await inviteTask.Should().NotThrowAsync();
            await acceptTask.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestInvitationAndRejectionAsync()
        {
            var userToInvite = await UpsertNewUserAsync();
            Func<Task> inviteTask = () => _channelClient.InviteAsync(_channel.Type, _channel.Id, userToInvite.Id);
            Func<Task> rejectTask = () => _channelClient.RejectInviteAsync(_channel.Type, _channel.Id, userToInvite.Id);

            await inviteTask.Should().NotThrowAsync();
            await rejectTask.Should().NotThrowAsync();
        }
    }
}