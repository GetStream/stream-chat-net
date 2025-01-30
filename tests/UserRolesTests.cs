using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests;

internal class UserRolesTests : TestBase
{
    private UserRequest _user1;
    private UserRequest _user2;

    [SetUp]
    public async Task SetupAsync()
    {
        (_user1, _user2) = (await UpsertNewUserAsync(), await UpsertNewUserAsync());
    }

    [TearDown]
    public async Task TeardownAsync()
    {
        await TryDeleteUsersAsync(_user1.Id, _user2.Id);
    }

    [Test]
    public async Task WhenUserIsAddedToChannelExpectChannelMemberRoleAssignedAsync()
    {
        var channel = await CreateChannelAsync(createdByUserId: _user1.Id);

        var addMembersResponse = await _channelClient.AddMembersAsync(channel.Type, channel.Id, new[] { _user2.Id });
        addMembersResponse.Members.First(m => m.UserId == _user2.Id).ChannelRole.Should()
            .BeEquivalentTo("channel_member");

        var getChannel = await _channelClient.GetOrCreateAsync(channel.Type, channel.Id, new ChannelGetRequest());

        getChannel.Members.First(m => m.UserId == _user2.Id).ChannelRole.Should().BeEquivalentTo("channel_member");
    }

    [Test]
    public async Task WhenAssigningARoleExpectRoleAssignedAsync()
    {
        var channel = await CreateChannelAsync(createdByUserId: _user1.Id);
        await _channelClient.AddMembersAsync(channel.Type, channel.Id, new[] { _user2.Id });

        var resp = await _channelClient.AssignRolesAsync(channel.Type, channel.Id, new AssignRoleRequest
        {
            AssignRoles = new List<RoleAssignment>
            {
                new RoleAssignment { UserId = _user2.Id, ChannelRole = Role.ChannelModerator },
            },
        });
        resp.Members.First().ChannelRole.Should().BeEquivalentTo("channel_moderator");
    }
}