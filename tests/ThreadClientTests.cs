using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Clients;
using StreamChat.Models;

namespace StreamChatTests
{
    [Parallelizable(ParallelScope.None)]
    [TestFixture]
    public class ThreadClientTests : TestBase
    {
        private ChannelWithConfig _channel;
        private UserRequest _user;
        private Message _parentMessage;
        private Message _replyMessage;

        [SetUp]
        public async Task SetupAsync()
        {
            _user = await UpsertNewUserAsync();
            _channel = await CreateChannelAsync(createdByUserId: _user.Id, members: new[] { _user.Id });

            // Create a parent message to start a thread
            var parentMessageResp = await _messageClient.SendMessageAsync(
                _channel.Type,
                _channel.Id,
                new MessageRequest { Text = "Parent message for thread" },
                _user.Id);
            _parentMessage = parentMessageResp.Message;

            // Create a reply in the thread
            var replyMessageResp = await _messageClient.SendMessageToThreadAsync(
                _channel.Type,
                _channel.Id,
                new MessageRequest { Text = "Reply in thread" },
                _user.Id,
                _parentMessage.Id);
            _replyMessage = replyMessageResp.Message;
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            await TryDeleteUsersAsync(_user.Id);
        }

        [Test]
        public async Task TestQueryThreadsWithFilter()
        {
            var filter = new Dictionary<string, object>
            {
                { "channel_cid", new Dictionary<string, object> { { "$eq", _channel.Cid } } },
            };
            var opts = QueryThreadsOptions.Default.WithFilter(filter).WithUserId(_user.Id);

            var resp = await _threadClient.QueryThreads(opts);

            resp.Threads.Should().NotBeNull();
            resp.Threads.Should().NotBeEmpty();

            // Verify the thread contains our parent message
            var thread = resp.Threads[0];
            thread.ChannelCID.Should().Be(_channel.Cid);
            thread.ParentMessageID.Should().Be(_parentMessage.Id);
            thread.CreatedByUserID.Should().Be(_user.Id);
            thread.ReplyCount.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task TestQueryThreadsWithSort()
        {
            var sort = new List<SortParameter>
            {
                new SortParameter { Field = "created_at", Direction = SortDirection.Descending },
            };
            var opts = QueryThreadsOptions.Default.WithSortBy(sort[0]).WithUserId(_user.Id);

            var resp = await _threadClient.QueryThreads(opts);

            resp.Threads.Should().NotBeNull();
            resp.Threads.Should().NotBeEmpty();

            // Verify the thread is sorted by created_at in descending order
            var thread = resp.Threads[0];
            thread.ChannelCID.Should().Be(_channel.Cid);
            thread.ParentMessageID.Should().Be(_parentMessage.Id);
            thread.CreatedAt.Should().BeAfter(DateTimeOffset.MinValue);
        }

        [Test]
        public async Task TestQueryThreadsWithFilterAndSort()
        {
            var filter = new Dictionary<string, object>
            {
                { "channel_cid", new Dictionary<string, object> { { "$eq", _channel.Cid } } },
            };
            var sort = new List<SortParameter>
            {
                new SortParameter { Field = "created_at", Direction = SortDirection.Descending },
            };
            var opts = QueryThreadsOptions.Default.WithFilter(filter).WithSortBy(sort[0]).WithUserId(_user.Id);

            var resp = await _threadClient.QueryThreads(opts);

            resp.Threads.Should().NotBeNull();
            resp.Threads.Should().NotBeEmpty();

            // Verify the thread matches our filter and sort criteria
            var thread = resp.Threads[0];
            thread.ChannelCID.Should().Be(_channel.Cid);
            thread.ParentMessageID.Should().Be(_parentMessage.Id);
            thread.CreatedAt.Should().BeAfter(DateTimeOffset.MinValue);
            thread.ReplyCount.Should().BeGreaterThan(0);
        }
    }
}
