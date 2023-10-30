using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="AppClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class AppClientTests : TestBase
    {
        private UserRequest _user1;
        private UserRequest _user2;
        private ChannelWithConfig _channel;
        private Message _message;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await _appClient.UpdateAppSettingsAsync(new AppSettingsRequest
            {
                PushConfig = new PushConfigRequest { Version = "v2" },
            });
        }

        [SetUp]
        public async Task SetUpAsync()
        {
            (_user1, _user2) = (await UpsertNewUserAsync(), await UpsertNewUserAsync());
            _channel = await CreateChannelAsync(_user1.Id, new[] { _user1.Id, _user2.Id });
            var resp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user1.Id, $"Hello @{_user2.Id}");
            _message = resp.Message;
        }

        [TearDown]
        public async Task Teardownsync()
        {
            await TryDeleteChannelAsync(_channel.Cid);
            await TryDeleteUsersAsync(_user1.Id, _user2.Id);
        }

        [Test]
        public async Task TestGetAppSettingsAsync()
        {
            var resp = await _appClient.GetAppSettingsAsync();

            resp.App.Should().NotBeNull();
        }

        [Test]
        public async Task TestUpdateAppSettings()
        {
            Func<Task> updateCall = () => _appClient.UpdateAppSettingsAsync(new AppSettingsRequest { ImageModerationEnabled = false });

            await updateCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestGetRateLimitAsync()
        {
            var rateLimit = await _appClient.GetRateLimitsAsync(new GetRateLimitsOptions().WithServerSide());

            rateLimit.ServerSide.Should().NotBeEmpty();
        }

        [Test]
        public async Task TestRevokeTokesAsync()
        {
            Func<Task> revokeCall = () => _appClient.RevokeTokensAsync(DateTimeOffset.UtcNow.AddHours(-1));

            await revokeCall.Should().NotThrowAsync();
        }

        [Test]
        public async Task TestTestPushAsync()
        {
            var resp = await _appClient.CheckPushAsync(new AppCheckPushRequest
            {
                MessageId = _message.Id,
                SkipDevices = true,
                UserId = _user2.Id,
            });

            resp.RenderedMessage.Should().NotBeNull();
        }

        [Test]
        public async Task TestTestSqsPushAsync()
        {
            var resp = await _appClient.CheckSqsPushAsync(new AppCheckSqsRequest
            {
                SqsKey = "key",
                SqsSecret = "secret",
                SqsUrl = "https://foo.com/bar",
            });

            resp.Status.Should().Be(SqsCheckStatus.Error);
        }

        [Test]
        public async Task TestTestSnsPushAsync()
        {
            var resp = await _appClient.CheckSnsPushAsync(new AppCheckSnsRequest
            {
                SnsKey = "key",
                SnsSecret = "secret",
                SnsTopicArn = "arn:aws:sns:us-east-1:123456789012:sns-topic",
            });

            resp.Status.Should().Be(SnsCheckStatus.Error);
        }
    }
}