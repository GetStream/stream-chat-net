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

        [SetUp]
        public async Task SetupAsync()
        {
            _user = await UpsertNewUserAsync();
            _channel = await CreateChannelAsync(createdByUserId: _user.Id, members: new[] { _user.Id });
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
            var opts = QueryThreadsOptions.Default.WithFilter(filter);

            var resp = await _threadClient.QueryThreads(opts);

            resp.Threads.Should().NotBeNull();
        }

        [Test]
        public async Task TestQueryThreadsWithSort()
        {
            var sort = new List<SortParameter>
            {
                new SortParameter { Field = "created_at", Direction = SortDirection.Descending },
            };
            var opts = QueryThreadsOptions.Default;
            opts.Sort = sort;

            var resp = await _threadClient.QueryThreads(opts);

            resp.Threads.Should().NotBeNull();
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
            var opts = QueryThreadsOptions.Default.WithFilter(filter);
            opts.Sort = sort;

            var resp = await _threadClient.QueryThreads(opts);

            resp.Threads.Should().NotBeNull();
        }
    }
}
