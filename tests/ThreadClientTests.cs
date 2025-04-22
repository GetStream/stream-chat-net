using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    [Parallelizable(ParallelScope.None)]
    [TestFixture]
    public class ThreadClientTests : TestBase
    {
        private ChannelWithConfig _channel;
        private UserRequest _user;
        private List<Message> _parentMessages = new List<Message>();
        private List<Message> _replyMessages = new List<Message>();

        [SetUp]
        public async Task SetupAsync()
        {
            _user = await UpsertNewUserAsync();
            _channel = await CreateChannelAsync(createdByUserId: _user.Id, members: new[] { _user.Id });

            for (int i = 1; i <= 3; i++)
            {
                var parentMessageResp = await _messageClient.SendMessageAsync(
                    _channel.Type,
                    _channel.Id,
                    new MessageRequest { Text = $"Parent message for thread {i}" },
                    _user.Id);
                var parentMessage = parentMessageResp.Message;
                _parentMessages.Add(parentMessage);

                var replyMessageResp = await _messageClient.SendMessageToThreadAsync(
                    _channel.Type,
                    _channel.Id,
                    new MessageRequest { Text = $"Reply in thread {i}" },
                    _user.Id,
                    parentMessage.Id);
                var replyMessage = replyMessageResp.Message;
                _replyMessages.Add(replyMessage);

                await Task.Delay(100);
            }
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

            var resp = await _threadClient.QueryThreadsAsync(opts);

            resp.Threads.Should().NotBeNull();
            resp.Threads.Should().NotBeEmpty();

            resp.Threads.Should().HaveCount(3);
            foreach (var thread in resp.Threads)
            {
                thread.ChannelCID.Should().Be(_channel.Cid);
            }
        }

        [Test]
        public async Task TestQueryThreadsWithSort()
        {
            var opts = QueryThreadsOptions.Default.WithSortBy(new SortParameter { Field = "created_at", Direction = SortDirection.Descending }).WithUserId(_user.Id);

            var resp = await _threadClient.QueryThreadsAsync(opts);

            resp.Threads.Should().NotBeNull();
            resp.Threads.Should().NotBeEmpty();
            resp.Threads.Should().HaveCountGreaterThanOrEqualTo(3);

            for (int i = 0; i < resp.Threads.Count - 1; i++)
            {
                resp.Threads[i].CreatedAt.Should().BeAfter(resp.Threads[i + 1].CreatedAt);
            }
        }

        [Test]
        public async Task TestQueryThreadsWithFilterAndSort()
        {
            var filter = new Dictionary<string, object>
            {
                { "channel_cid", new Dictionary<string, object> { { "$eq", _channel.Cid } } },
            };
            var opts = QueryThreadsOptions.Default.WithFilter(filter).WithSortBy(new SortParameter { Field = "created_at", Direction = SortDirection.Descending }).WithUserId(_user.Id);

            var resp = await _threadClient.QueryThreadsAsync(opts);

            resp.Threads.Should().NotBeNull();
            resp.Threads.Should().NotBeEmpty();
            resp.Threads.Should().HaveCount(3);

            for (int i = 0; i < resp.Threads.Count - 1; i++)
            {
                resp.Threads[i].CreatedAt.Should().BeAfter(resp.Threads[i + 1].CreatedAt);
            }

            foreach (var thread in resp.Threads)
            {
                thread.ChannelCID.Should().Be(_channel.Cid);
            }
        }

        [Test]
        public async Task TestQueryThreadsWithPagination()
        {
            var firstPageOpts = QueryThreadsOptions.Default
                .WithUserId(_user.Id)
                .WithLimit(2)
                .WithSortBy(new SortParameter { Field = "created_at", Direction = SortDirection.Descending })
                .WithFilter(new Dictionary<string, object> { { "channel_cid", new Dictionary<string, object> { { "$eq", _channel.Cid } } } });

            var firstPageResp = await _threadClient.QueryThreadsAsync(firstPageOpts);

            firstPageResp.Threads.Should().HaveCount(2);
            firstPageResp.Next.Should().NotBeNullOrEmpty();

            var secondPageOpts = QueryThreadsOptions.Default
                .WithUserId(_user.Id)
                .WithLimit(2)
                .WithSortBy(new SortParameter { Field = "created_at", Direction = SortDirection.Descending })
                .WithFilter(new Dictionary<string, object> { { "channel_cid", new Dictionary<string, object> { { "$eq", _channel.Cid } } } })
                .WithNext(firstPageResp.Next);

            var secondPageResp = await _threadClient.QueryThreadsAsync(secondPageOpts);

            secondPageResp.Threads.Should().HaveCount(1);

            var allThreads = firstPageResp.Threads.Concat(secondPageResp.Threads).ToList();
            allThreads.Should().HaveCount(3);

            for (int i = 0; i < allThreads.Count - 1; i++)
            {
                allThreads[i].CreatedAt.Should().BeAfter(allThreads[i + 1].CreatedAt);
            }
        }

        [Test]
        public async Task TestQueryThreadsWithoutFilterAndSort()
        {
            var opts = QueryThreadsOptions.Default.WithUserId(_user.Id);

            var resp = await _threadClient.QueryThreadsAsync(opts);

            resp.Threads.Should().NotBeNull();
            resp.Threads.Should().NotBeEmpty();

            resp.Threads.Should().HaveCountGreaterThanOrEqualTo(3);
        }
    }
}
