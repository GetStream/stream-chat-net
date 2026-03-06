Channel members are users who have been added to a channel and can participate in conversations. This page covers how to manage channel membership, including adding and removing members, controlling message history visibility, and managing member roles.

## Adding and Removing Members

### Adding Members

Using the `addMembers()` method adds the given users as members to a channel.

```csharp
await channelClient.AddMembersAsync("channel-type", "channel-id", "thierry", "josh");
```

> [!NOTE]
> **Note:** You can only add/remove up to 100 members at once.


Members can also be added when creating a channel:

```csharp
// Members can be added by passing an array of user IDs
var response = await _channelClient.GetOrCreateAsync("messaging", "my-channel-id", createdBy: "user-1",
    members: new[] { "user-2", "user-3" });
var channel = response.Channel;
```

### Removing Members

Using the `removeMembers()` method removes the given users from the channel.

```csharp
await channelClient.RemoveMembersAsync("channel-type", "channel-id", new[] { "thierry", "josh" });
```

### Leaving a Channel

Users can leave a channel without moderator-level permissions. Ensure channel members have the `Leave Own Channel` permission enabled.

```csharp
await channel.RemoveMembersAsync(member);
```

> [!NOTE]
> You can familiarize yourself with all permissions in the [Permissions section](/chat/docs/dotnet-csharp/chat_permission_policies/).


## Hide History

When members join a channel, you can specify whether they have access to the channel's message history. By default, new members can see the history. Set `hide_history` to `true` to hide it for new members.

```csharp
await channelClient.AddMembersAsync("channel-type", "channel-id", new[] {"thierry"}, null, new AddMemberOptions { HideHistory = true })
```

### Hide History Before a Specific Date

Alternatively, `hide_history_before` can be used to hide any history before a given timestamp while giving members access to later messages. The value must be a timestamp in the past in RFC 3339 format. If both parameters are defined, `hide_history_before` takes precedence over `hide_history`.

```csharp
var cutoff = DateTimeOffset.UtcNow.AddDays(-7); // Last 7 days
await channelClient.AddMembersAsync("channel-type", "channel-id", new[] {"thierry"}, null, new AddMemberOptions { HideHistoryBefore = cutoff })
```

## System Message Parameter

You can optionally include a message object when adding or removing members that client-side SDKs will use to display a system message. This works for both adding and removing members.

```csharp
var msg = new MessageRequest { Text: "Tommaso joined the channel", UserId: "tommaso" };
await channelClient.AddMembersAsync("channel-type", "channel-id", new[] {"tommaso"}, msg, null);
```

## Adding and Removing Moderators

Using the `addModerators()` method adds the given users as moderators (or updates their role to moderator if already members), while `demoteModerators()` removes the moderator status.

### Add Moderators

```csharp
await channelClient.AddModeratorsAsync("channel-type", "channel-type", new[] { "thierry", "josh" });
```

### Remove Moderators

```csharp
await channelClient.DemoteModeratorsAsync("channel-type", "channel-type", new[] { "tommaso" });
```

> [!NOTE]
> These operations can only be performed server-side, and a maximum of 100 moderators can be added or removed at once.


## Member Custom Data

Custom data can be added at the channel member level. This is useful for storing member-specific information that is separate from user-level data. Ensure custom data does not exceed 5KB.

### Adding Custom Data

```csharp
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
```

### Updating Member Data

Channel members can be partially updated. Only custom data and channel roles are eligible for modification. You can set or unset fields, either separately or in the same call.

```csharp
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

// Set and unset in a single request
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
```
