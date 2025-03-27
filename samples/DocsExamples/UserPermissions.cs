using StreamChat.Clients;
using StreamChat.Models;

namespace DocsExamples;

/// <summary>
/// Code examples for <see href="https://getstream.io/chat/docs/python/user_permissions/"/>
/// </summary>
internal class UserPermissions
{
    private readonly IUserClient _userClient;
    private readonly IChannelClient _channelClient;
    private readonly IPermissionClient _permissionClient;
    private readonly IChannelTypeClient _channelTypeClient;
    private readonly IAppClient _appClient;

    public UserPermissions()
    {
        var factory = new StreamClientFactory("{{ api_key }}", "{{ api_secret }}");
        _userClient = factory.GetUserClient();
        _channelClient = factory.GetChannelClient();
        _permissionClient = factory.GetPermissionClient();
        _channelTypeClient = factory.GetChannelTypeClient();
        _appClient = factory.GetAppClient();
    }

    internal async Task ChangeUserRole()
    {
        var upsertResponse = await _userClient.UpdatePartialAsync(new UserPartialRequest
        {
            Id = "user-id",
            Set = new Dictionary<string, object>
            {
                { "role", "special_agent" }
            }
        });
    }

    internal async Task VerifyChannelMemberRoleAssigned()
    {
        var addMembersResponse
            = await _channelClient.AddMembersAsync("channel-type", "channel-id", new[] { "user-id" });
        Console.WriteLine(addMembersResponse.Members[0].ChannelRole); // channel role is equal to "channel_member"
    }

    internal async Task AssignRoles()
    {
        // User must be a member of the channel before you can assign channel role
        var resp = await _channelClient.AssignRolesAsync("channel-type", "channel-id", new AssignRoleRequest
        {
            AssignRoles = new List<RoleAssignment>
            {
                new RoleAssignment { UserId = "user-id", ChannelRole = Role.ChannelModerator }
            }
        });
    }

    internal async Task CreateRole()
    {
        await _permissionClient.CreateRoleAsync("special_agent");
    }

    internal async Task DeleteRole()
    {
        await _permissionClient.DeleteRoleAsync("special_agent");
    }

    internal async Task ListPermissions()
    {
        var response = await _permissionClient.ListPermissionsAsync();
    }

    internal async Task UpdateGrantedPermissions()
    {
        // observe current grants of the channel type
        var channelType = await _channelTypeClient.GetChannelTypeAsync("messaging");
        Console.WriteLine(channelType.Grants);

        // update "channel_member" role grants in "messaging" scope
        var update = new ChannelTypeWithStringCommandsRequest
        {
            Grants = new Dictionary<string, List<string>>
            {
                {
                    // This will replace all existing grants of "channel_member" role
                    "channel_member", new List<string>
                    {
                        "read-channel", // allow access to the channel
                        "create-message", // create messages in the channel
                        "update-message-owner", // update own user messages
                        "delete-message-owner", // delete own user messages
                    }
                },
            }
        };
        await _channelTypeClient.UpdateChannelTypeAsync("messaging", update);
    }

    internal async Task RemoveGrantedPermissionsByCategory()
    {
        var update = new ChannelTypeWithStringCommandsRequest
        {
            Grants = new Dictionary<string, List<string>>
            {
                { "guest", new List<string>() }, // removes all grants of "guest" role
                { "anonymous", new List<string>() }, // removes all grants of "anonymous" role
            }
        };
        await _channelTypeClient.UpdateChannelTypeAsync("messaging", update);
    }

    internal async Task ResetGrantsToDefaultSettings()
    {
        var update = new ChannelTypeWithStringCommandsRequest
        {
            Grants = new Dictionary<string, List<string>>()
        };
        await _channelTypeClient.UpdateChannelTypeAsync("messaging", update);
    }

    internal async Task UpdateAppScopedGrants()
    {
        var settings = new AppSettingsRequest
        {
            Grants = new Dictionary<string, List<string>>
            {
                { "anonymous", new List<string>() },
                { "guest", new List<string>() },
                { "user", new List<string> { "search-user", "mute-user" } },
                { "admin", new List<string> { "search-user", "mute-user", "ban-user" } },
            }
        };
        await _appClient.UpdateAppSettingsAsync(settings);
    }

    internal async Task UpdateChannelLevelPermissions()
    {
        var grants = new Dictionary<string, object> { { "user", new List<string> { "!add-links", "create-reaction" } } };
        var overrides = new Dictionary<string, object> { { "grants", grants } };
        var request = new PartialUpdateChannelRequest
        {
            Set = new Dictionary<string, object>
            {
                { "config_overrides", overrides }
            }
        };
        var resp = await _channelClient.PartialUpdateAsync("channel-type", "channel-id", request);
    }
}