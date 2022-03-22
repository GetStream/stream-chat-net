using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="FlagClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class FlagClientTests : TestBase
    {
        private ChannelWithConfig _channel;
        private UserRequest _channelCreatorUser;
        private UserRequest _chanMember1;
        private UserRequest _chanMember2;
        private Message _message;

        [OneTimeSetUp]
        public async Task OneTimeSetupAsync()
        {
            _channelCreatorUser = await UpsertNewUserAsync();
            _channel = await CreateChannelAsync(_channelCreatorUser.Id, new[] { _channelCreatorUser.Id });
        }

        [OneTimeTearDown]
        public async Task OneTimeTeardownAsync()
        {
            await TryDeleteChannelAsync(_channel.Cid);
            await TryDeleteUsersAsync(_channelCreatorUser.Id);
        }

        [SetUp]
        public async Task SetupAsync()
        {
            (_chanMember1, _chanMember2) = (await UpsertNewUserAsync(), await UpsertNewUserAsync());
            await _channelClient.AddMembersAsync(_channel.Type, _channel.Id, _chanMember1.Id, _chanMember2.Id);
            var resp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _chanMember1.Id, "Hello");
            _message = resp.Message;
        }

        [TearDown]
        public async Task TeardownAsync()
        {
            await TryDeleteUsersAsync(_chanMember1.Id, _chanMember2.Id);
        }

        [Test]
        public async Task TestFlagUserAsync()
        {
            Func<Task> flagCall = () => _flagClient.FlagUserAsync(_chanMember1.Id, _chanMember2.Id);

            await flagCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestQueryFlagsAsync()
        {
            await _flagClient.FlagMessageAsync(_message.Id, _chanMember1.Id);

            await WaitForAsync(async () =>
            {
                var resp = await _flagClient.QueryMessageFlags(new FlagMessageQueryRequest
                {
                    FilterConditions = new Dictionary<string, object>()
                    {
                        { "user_id", new Dictionary<string, object> { { "$in", new[] { _chanMember1.Id } } } },
                    },
                    Limit = 1,
                });

                return resp.Flags.Count > 0;
            });
        }

        [Test]
        public async Task TestQueryFlagAndReviewFlagAsync()
        {
            await _flagClient.FlagMessageAsync(_message.Id, _chanMember2.Id);

            var queryResp = await _flagClient.QueryFlagReportsAsync(new QueryFlagReportsRequest
            {
                FilterConditions = new Dictionary<string, object>
                {
                    { "message_id", _message.Id },
                },
            });
            var report = queryResp.FlagReports[0];
            report.Message.Id.Should().Be(_message.Id);

            var reviewResp = await _flagClient.ReviewFlagReportAsync(report.Id, new ReviewFlagReportRequest
            {
                ReviewResult = "reviewed",
                UserId = _chanMember1.Id,
            });
            reviewResp.FlagReport.ReviewResult.Should().Be("reviewed");
        }
    }
}