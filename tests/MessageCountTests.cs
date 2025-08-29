using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    [TestFixture]
    public class MessageCountTests : TestBase
    {
        private UserRequest _user;
        private ChannelWithConfig _channel;
        private ChannelWithConfig _channel_2;

        [SetUp]
        public async Task SetUp()
        {
            _user = await UpsertNewUserAsync();
            _channel = await CreateChannelAsync(_user.Id, autoDelete: false);
            _channel_2 = await CreateChannelAsync(_user.Id, autoDelete: false);
        }

        [TearDown]
        public async Task TearDown()
        {
            await TryDeleteChannelAsync(_channel.Cid);
            await TryDeleteChannelAsync(_channel_2.Cid);
            await TryDeleteUsersAsync(_user.Id);
        }

        [Test]
        public async Task TestMessageCountEnabledAsync()
        {
            await WaitForAsync(async () =>
            {
                var chanState = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id,
                    new ChannelGetRequest { State = true });
                return chanState.Channel.MessageCount == 1;
            });

            var finalState = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id,
                new ChannelGetRequest { State = true });
            finalState.Channel.MessageCount.Should().Be(1);
        }

        [Test]
        public async Task TestMessageCountDisabledAsync()
        {
            var updateRequest = new PartialUpdateChannelRequest
            {
                Set = new Dictionary<string, object>
                {
                    {
                        "config_overrides", new Dictionary<string, object>
                        {
                            { "count_messages", false },
                        }
                    },
                },
            };

            await _channelClient.PartialUpdateAsync(_channel_2.Type, _channel_2.Id, updateRequest);
            await _messageClient.SendMessageAsync(_channel_2.Type, _channel_2.Id, _user.Id, "hello");
            await WaitForAsync(async () =>
            {
                var state = await _channelClient.GetOrCreateAsync(_channel_2.Type, _channel_2.Id,
                    new ChannelGetRequest { State = true });
                return state.Channel.MessageCount == null;
            });

            var finalState = await _channelClient.GetOrCreateAsync(_channel_2.Type, _channel_2.Id,
                new ChannelGetRequest { State = true });
            finalState.Channel.MessageCount.Should().BeNull();
        }
    }
}
