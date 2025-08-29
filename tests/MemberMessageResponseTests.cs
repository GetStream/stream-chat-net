using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests;

internal class MemberMessageResponseTests : TestBase
{
    private UserRequest _user1;
    private UserRequest _user2;
    private ChannelWithConfig _channel;

    [SetUp]
    public async Task SetupAsync()
    {
        (_user1, _user2) = (await UpsertNewUserAsync(), await UpsertNewUserAsync());
        _channel = await CreateChannelAsync(createdByUserId: _user1.Id, members: new[] { _user1.Id, _user2.Id });
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await TryDeleteUsersAsync(_user1.Id, _user2.Id);
    }

    [Test]
    public async Task WhenSendingMessagesExpectChannelRoleIncludedAsync()
    {
        // Assign a custom channel role to _user2
        await _channelClient.AssignRolesAsync(_channel.Type, _channel.Id, new AssignRoleRequest
        {
            AssignRoles = new List<RoleAssignment>
            {
                new RoleAssignment { UserId = _user2.Id, ChannelRole = "custom_role" },
            },
        });

        // _user1 sends a message
        var user1MessageResp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user1.Id, "Hello from user1");

        user1MessageResp.Message.Member.Should().NotBeNull();
        user1MessageResp.Message.Member.UserId.Should().Be(_user1.Id);
        user1MessageResp.Message.Member.ChannelRole.Should().NotBeNullOrEmpty();

        // _user2 sends a message
        var user2MessageResp = await _messageClient.SendMessageAsync(_channel.Type, _channel.Id, _user2.Id, "Hello from user2");

        user2MessageResp.Message.Member.Should().NotBeNull();
        user2MessageResp.Message.Member.UserId.Should().Be(_user2.Id);
        user2MessageResp.Message.Member.ChannelRole.Should().Be("custom_role");

        // Fetch channel state and verify both messages include the correct creator role
        var channelState = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, new ChannelGetRequest { State = true, Watch = false });

        var fetchedUser1Msg = channelState.Messages.First(m => m.Id == user1MessageResp.Message.Id);
        fetchedUser1Msg.Member.Should().NotBeNull();
        fetchedUser1Msg.Member.ChannelRole.Should().Be(user1MessageResp.Message.Member.ChannelRole);

        var fetchedUser2Msg = channelState.Messages.First(m => m.Id == user2MessageResp.Message.Id);
        fetchedUser2Msg.Member.Should().NotBeNull();
        fetchedUser2Msg.Member.ChannelRole.Should().Be("custom_role");
    }
}
