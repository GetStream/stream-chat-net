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
        [Test]
        public async Task TestMessageCountEnabledAsync()
        {
            var user = await UpsertNewUserAsync();

            // Arrange – channel created with default settings (count_messages enabled)
            var channel = await CreateChannelAsync(user.Id);

            // Act – send a single message
            await _messageClient.SendMessageAsync(channel.Type, channel.Id, user.Id, "hello");

            // Assert – wait until API reflects message_count == 1
            await WaitForAsync(async () =>
            {
                var chanState = await _channelClient.GetOrCreateAsync(channel.Type, channel.Id,
                    new ChannelGetRequest { State = true });
                return chanState.Channel.MessageCount == 1;
            });

            var finalState = await _channelClient.GetOrCreateAsync(channel.Type, channel.Id,
                new ChannelGetRequest { State = true });
            finalState.Channel.MessageCount.Should().Be(1);
        }

        [Test]
        public async Task TestMessageCountDisabledAsync()
        {
            var user = await UpsertNewUserAsync();

            // Arrange – create channel with defaults first
            var channel = await CreateChannelAsync(user.Id);

            // Disable counting via partial update
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

            await _channelClient.PartialUpdateAsync(channel.Type, channel.Id, updateRequest);

            // Act – send a message
            await _messageClient.SendMessageAsync(channel.Type, channel.Id, user.Id, "hello");

            // Assert – message_count should remain null/absent
            await WaitForAsync(async () =>
            {
                var state = await _channelClient.GetOrCreateAsync(channel.Type, channel.Id,
                    new ChannelGetRequest { State = true });
                return state.Channel.MessageCount == null;
            });

            var finalState = await _channelClient.GetOrCreateAsync(channel.Type, channel.Id,
                new ChannelGetRequest { State = true });
            finalState.Channel.MessageCount.Should().BeNull();
        }
    }
}
