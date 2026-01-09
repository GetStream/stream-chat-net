using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Clients;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="ChannelBatchUpdater"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class ChannelBatchUpdaterTests : TestBase
    {
        [Test]
        public async Task TestUpdateChannelsBatchWithValidOptionsAsync()
        {
            var ch1 = await CreateChannelAsync(createdByUserId: (await UpsertNewUserAsync()).Id);
            var ch2 = await CreateChannelAsync(createdByUserId: (await UpsertNewUserAsync()).Id);
            var userToAdd = await UpsertNewUserAsync();

            var filter = new ChannelsBatchFilters
            {
                Cids = new Dictionary<string, object>
                {
                    { "$in", new[] { ch1.Cid, ch2.Cid } },
                },
            };

            var options = new ChannelsBatchOptions
            {
                Operation = ChannelBatchOperation.AddMembers,
                Filter = filter,
                Members = new[] { new ChannelBatchMemberRequest { UserId = userToAdd.Id } },
            };

            var response = await _channelClient.UpdateChannelsBatchAsync(options);

            response.TaskId.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task TestChannelBatchUpdaterAddMembersAsync()
        {
            var user1 = await UpsertNewUserAsync();
            var ch1 = await CreateChannelAsync(createdByUserId: user1.Id);
            var ch2 = await CreateChannelAsync(createdByUserId: user1.Id);

            var user2 = await UpsertNewUserAsync();
            var user3 = await UpsertNewUserAsync();

            var filter = new ChannelsBatchFilters
            {
                Cids = new Dictionary<string, object>
                {
                    { "$in", new[] { ch1.Cid, ch2.Cid } },
                },
            };

            var members = new[]
            {
                new ChannelBatchMemberRequest { UserId = user2.Id },
                new ChannelBatchMemberRequest { UserId = user3.Id },
            };

            var updater = _channelClient.BatchUpdater();
            var response = await updater.AddMembersAsync(filter, members);

            response.TaskId.Should().NotBeNullOrEmpty();
            await WaitUntilTaskSucceedsAsync(response.TaskId);

            var ch1Members = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = ch1.Type,
                Id = ch1.Id,
                FilterConditions = new Dictionary<string, object>(),
            });

            var ch2Members = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = ch2.Type,
                Id = ch2.Id,
                FilterConditions = new Dictionary<string, object>(),
            });

            ch1Members.Members.Should().Contain(m => m.UserId == user2.Id);
            ch1Members.Members.Should().Contain(m => m.UserId == user3.Id);
            ch2Members.Members.Should().Contain(m => m.UserId == user2.Id);
            ch2Members.Members.Should().Contain(m => m.UserId == user3.Id);
        }

        [Test]
        public async Task TestChannelBatchUpdaterRemoveMembersAsync()
        {
            var user1 = await UpsertNewUserAsync();
            var user2 = await UpsertNewUserAsync();
            var user3 = await UpsertNewUserAsync();

            var ch1 = await CreateChannelAsync(createdByUserId: user1.Id, members: new[] { user1.Id, user2.Id, user3.Id });
            var ch2 = await CreateChannelAsync(createdByUserId: user1.Id, members: new[] { user1.Id, user2.Id, user3.Id });

            var filter = new ChannelsBatchFilters
            {
                Cids = new Dictionary<string, object>
                {
                    { "$in", new[] { ch1.Cid, ch2.Cid } },
                },
            };

            var members = new[]
            {
                new ChannelBatchMemberRequest { UserId = user2.Id },
            };

            var updater = _channelClient.BatchUpdater();
            var response = await updater.RemoveMembersAsync(filter, members);

            response.TaskId.Should().NotBeNullOrEmpty();
            await WaitUntilTaskSucceedsAsync(response.TaskId);

            var ch1Members = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = ch1.Type,
                Id = ch1.Id,
                FilterConditions = new Dictionary<string, object>(),
            });

            var ch2Members = await _channelClient.QueryMembersAsync(new QueryMembersRequest
            {
                Type = ch2.Type,
                Id = ch2.Id,
                FilterConditions = new Dictionary<string, object>(),
            });

            ch1Members.Members.Should().NotContain(m => m.UserId == user2.Id);
            ch2Members.Members.Should().NotContain(m => m.UserId == user2.Id);
        }

        [Test]
        public async Task TestChannelBatchUpdaterArchiveAsync()
        {
            var user1 = await UpsertNewUserAsync();
            var user2 = await UpsertNewUserAsync();

            var ch1 = await CreateChannelAsync(createdByUserId: user1.Id, members: new[] { user1.Id, user2.Id });
            var ch2 = await CreateChannelAsync(createdByUserId: user1.Id, members: new[] { user1.Id, user2.Id });

            var filter = new ChannelsBatchFilters
            {
                Cids = new Dictionary<string, object>
                {
                    { "$in", new[] { ch1.Cid, ch2.Cid } },
                },
            };

            var members = new[]
            {
                new ChannelBatchMemberRequest { UserId = user1.Id },
            };

            var updater = _channelClient.BatchUpdater();
            var response = await updater.ArchiveAsync(filter, members);

            response.TaskId.Should().NotBeNullOrEmpty();
            await WaitUntilTaskSucceedsAsync(response.TaskId);

            var ch1State = await _channelClient.GetOrCreateAsync(ch1.Type, ch1.Id, new ChannelGetRequest());
            var ch1Member = ch1State.Members.FirstOrDefault(m => m.UserId == user1.Id);

            ch1Member.Should().NotBeNull();
            ch1Member.ArchivedAt.Should().NotBeNull();
        }
    }
}
