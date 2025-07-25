using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat;
using StreamChat.Exceptions;
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

        [Test]
        public async Task TestReadingAppGrantsAsync()
        {
            var resp = await _appClient.GetAppSettingsAsync();
            resp.App.Grants.Should().NotBeNull();
        }

        [Test]
        public async Task TestWritingAppGrantsAsync()
        {
            var getAppResponse = await _appClient.GetAppSettingsAsync();
            var userGrants = GetUserGrants(getAppResponse.App.Grants);

            // Remove permission
            userGrants.Remove("delete-poll-owner");
            await _appClient.UpdateAppSettingsAsync(new AppSettingsRequest { Grants = getAppResponse.App.Grants });

            // Assert permissions is removed
            var getAppResponse2 = await _appClient.GetAppSettingsAsync();
            var userGrants2 = GetUserGrants(getAppResponse2.App.Grants);
            userGrants2.Should().NotContain("delete-poll-owner");

            // Add permission
            userGrants2.Add("delete-poll-owner");
            await _appClient.UpdateAppSettingsAsync(new AppSettingsRequest { Grants = getAppResponse2.App.Grants });

            // Assert permissions is added
            var getAppResponse3 = await _appClient.GetAppSettingsAsync();
            var userGrants3 = GetUserGrants(getAppResponse3.App.Grants);
            userGrants3.Should().Contain("delete-poll-owner");

            return;

            List<string> GetUserGrants(Dictionary<string, List<string>> grants)
                => grants.First(g => g.Key == "user").Value;
        }

        [Test]
        public async Task TestEventHooksAsync()
        {
            var webhookHook = new EventHook
            {
                Id = "95fa9371-38d8-4ddb-b099-d9ed86e7c9bc",
                HookType = HookType.Webhook,
                Enabled = true,
                EventTypes = new List<string> { "message.new", "message.updated" },
                WebhookUrl = "https://example.com/webhook",
            };

            var sqsHook = new EventHook
            {
                Id = "4eaa795f-77d2-4b72-8f7e-11de0327121c",
                HookType = HookType.SQS,
                Enabled = true,
                EventTypes = new List<string> { "user.updated" },
                SqsQueueUrl = "https://sqs.region.amazonaws.com/123456789012/queue-name",
                SqsRegion = "us-east-1",
                SqsAuthType = AuthType.Resource,
            };

            var snsHook = new EventHook
            {
                Id = "7b2c6590-7b61-490a-8987-96c5f8a353ca",
                HookType = HookType.SNS,
                Enabled = true,
                EventTypes = new List<string> { "channel.updated" },
                SnsTopicArn = "arn:aws:sns:us-east-1:123456789012:topic-name",
                SnsRegion = "us-east-1",
                SnsAuthType = AuthType.Resource,
            };

            var eventHooks = new List<EventHook> { webhookHook, sqsHook, snsHook };

            await _appClient.UpdateAppSettingsAsync(new AppSettingsRequest { EventHooks = eventHooks });

            var getAppResponse = await _appClient.GetAppSettingsAsync();
            getAppResponse.App.EventHooks.Should().NotBeNull();
            getAppResponse.App.EventHooks.Should().HaveCount(3);
            getAppResponse.App.EventHooks.Should().BeEquivalentTo(eventHooks, options => options.Excluding(e => e.CreatedAt).Excluding(e => e.UpdatedAt));
        }
    }
}
