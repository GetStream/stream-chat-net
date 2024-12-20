using StreamChat.Clients;
using StreamChat.Models;

namespace DocsExamples;

/// <summary>
/// Code examples for <see href="https://getstream.io/chat/docs/dotnet-csharp/channel_member/"/>
/// </summary>
internal class ChannelMembers
{
    private readonly IChannelClient _channelClient;

    public ChannelMembers()
    {
        var factory = new StreamClientFactory("{{ api_key }}", "{{ api_secret }}");
        _channelClient = factory.GetChannelClient();
    }

    public static void Main(string[] args)
    {
    }

    internal async Task AddChannelMembersWhenCreatingChannelAsync()
    {
        // members can be added by passing an array of user IDs
        var response = await _channelClient.GetOrCreateAsync("messaging", "my-channel-id", createdBy: "user-1",
            members: new[] { "user-2", "user-3" });
        var channel = response.Channel;

        // or by passing objects
        var response2 = await _channelClient.GetOrCreateAsync("messaging", new ChannelGetRequest
        {
            Data = new ChannelRequest
            {
                CreatedBy = new UserRequest { Id = "user-1" },
                Members = new List<ChannelMember>
                {
                    new() { UserId = "user-2" },
                    new() { UserId = "user-3" },
                }
            },
        });
        var channel2 = response2.Channel;
    }

    internal async Task AddChannelMembersByUsingAddMembersAsync()
    {
        await _channelClient.AddMembersAsync("messaging", "my-channel-id", new[] { "user-2", "user-3" });
    }

    internal async Task AddChannelMemberWithMessageAsync()
    {
        await _channelClient.AddMembersAsync("messaging", "my-channel-id", new[] { "user-2" }, new MessageRequest
        {
            Text = "Tommaso joined the channel.",
            UserId = "user-1", // Message sender
        }, options: null);
    }

    internal async Task AddChannelMemberWithHideHistoryAsync()
    {
        await _channelClient.AddMembersAsync("messaging", "my-channel-id", new[] { "user-2" }, msg: null,
            new AddMemberOptions
            {
                HideHistory = true
            });
    }

    internal async Task AddChannelMemberWithCustomDataAsync()
    {
        // TODO: add AddMembersAsync overload that accepts custom data
        await _channelClient.AddMembersAsync("messaging", "my-channel-id", new[] { "user-2" });

        var partialRequest = new ChannelMemberPartialRequest
        {
            UserId = "user-2",
            Set = new Dictionary<string, object>
            {
                { "hat", "blue" }, // Channel member custom data is separate from user custom data
            },
        };
        await _channelClient.UpdateMemberPartialAsync("messaging", "my-channel-id", partialRequest);
    }

    internal async Task RemoveChannelMembersAsync()
    {
        await _channelClient.RemoveMembersAsync("messaging", "my-channel-id", new[] { "user-2", "user-3" });
    }

    internal async Task RemoveOwnChannelMembershipAsync()
    {
        await _channelClient.RemoveMembersAsync("messaging", "my-channel-id", new[] { "user-2" });
    }

    internal async Task AddModeratorsAsync()
    {
        await _channelClient.AddModeratorsAsync("messaging", "my-channel-id", new[] { "user-2", "user-3" });
    }

    internal async Task RemoveModeratorsAsync()
    {
        await _channelClient.DemoteModeratorsAsync("messaging", "my-channel-id", new[] { "user-2", "user-3" });
    }

    internal async Task UpdateChannelMembersAsync()
    {
        // Set some fields
        var memberResponse = await _channelClient.UpdateMemberPartialAsync("messaging", "my-channel-id",
            new ChannelMemberPartialRequest
            {
                UserId = "user-2",
                Set = new Dictionary<string, object>
                {
                    { "hat", "blue" },
                    { "score", 1000 },
                },
            });

        // Unset some fields
        var memberResponse2 = await _channelClient.UpdateMemberPartialAsync("messaging", "my-channel-id",
            new ChannelMemberPartialRequest
            {
                UserId = "user-2",
                Unset = new[] { "hat", "score" },
            });

        // Set / Unset in a single request
        var memberResponse3 = await _channelClient.UpdateMemberPartialAsync("messaging", "my-channel-id",
            new ChannelMemberPartialRequest
            {
                UserId = "user-2",
                Set = new Dictionary<string, object>
                {
                    { "hat", "blue" },
                },
                Unset = new[] { "score" },
            });
    }

    internal async Task PaginationAndOrderingAsync()
    {
        // Simple members query
        var response = await _channelClient.QueryMembersAsync(new QueryMembersRequest
        {
            Type = "messaging",
            Id = "my-channel-id",
            FilterConditions = new Dictionary<string, object>(),
        });

        // Query members with filter and sort
        var response2 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
        {
            Type = "messaging",
            Id = "my-channel-id",
            FilterConditions = new Dictionary<string, object>
            {
                { "color", "blue" }, // Filters by member custom data (not user custom data)
            },
            Sorts = new[]
            {
                new SortParameter
                {
                    Field = "created_at",
                    Direction = SortDirection.Descending,
                },
            },
        });

        // Query members with limit and pagination
        var response3 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
        {
            Type = "messaging",
            Id = "my-channel-id",
            FilterConditions = new Dictionary<string, object>
            {
                { "color", "blue" }, // Filters by member custom data (not user custom data)
            },
            Limit = 50, // Take 50 records
            Offset = 50, // Skip the first 50 records. Use for pagination -> calc from limit & page
            Sorts = new[]
            {
                new SortParameter
                {
                    Field = "created_at",
                    Direction = SortDirection.Descending,
                },
            },
        });
    }

    internal async Task FilteringAsync()
    {
        // Get members with pending invites
        var response = await _channelClient.QueryMembersAsync(new QueryMembersRequest
        {
            Type = "messaging",
            Id = "my-channel-id",
            FilterConditions = new Dictionary<string, object>
            {
                { "invite", "pending" },
            },
        });

        // Search by name autocomplete
        var response2 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
        {
            Type = "messaging",
            Id = "my-channel-id",
            FilterConditions = new Dictionary<string, object>
            {
                {
                    "name", new Dictionary<string, string>
                    {
                        { "$autocomplete", "tomm" }
                    }
                },
            },
        });

        // Get moderators
        var response3 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
        {
            Type = "messaging",
            Id = "my-channel-id",
            FilterConditions = new Dictionary<string, object>
            {
                { "channel_role", "channel_moderator" },
            },
        });

        // Get members who have joined the channel
        var response4 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
        {
            Type = "messaging",
            Id = "my-channel-id",
            FilterConditions = new Dictionary<string, object>
            {
                { "joined", true },
            },
        });

        // Get banned members
        var response5 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
        {
            Type = "messaging",
            Id = "my-channel-id",
            FilterConditions = new Dictionary<string, object>
            {
                { "banned", true },
            },
        });

        // Get members by custom data
        var response6 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
        {
            Type = "messaging",
            Id = "my-channel-id",
            FilterConditions = new Dictionary<string, object>
            {
                { "subscription", "gold_plan" },
            },
        });

        // Get members by user email
        var response7 = await _channelClient.QueryMembersAsync(new QueryMembersRequest
        {
            Type = "messaging",
            Id = "my-channel-id",
            FilterConditions = new Dictionary<string, object>
            {
                { "user.email", "example@getstream.io" },
            },
        });
    }
}