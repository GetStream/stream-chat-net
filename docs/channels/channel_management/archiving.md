Channel members can archive a channel for themselves. This is a per-user setting that does not affect other members.

Archived channels function identically to regular channels via the API, but your UI can display them separately. When a channel is archived, the timestamp is recorded and returned as `archived_at` in the response.

When querying channels, filter by `archived: true` to retrieve only archived channels, or `archived: false` to exclude them.

## Archive a Channel

```csharp
// Archive
var archiveResponse = await _channelClient.ArchiveAsync("messaging", "channel-id", "user-id");

// Get the date when the channel got archived by the user
var archivedAt = archiveResponse.ChannelMember.ArchivedAt;

// Get channels that are NOT archived
var unarchivedChannels = await _channelClient.QueryChannelsAsync(new QueryChannelsOptions
{
    Filter = new Dictionary<string, object>
    {
        { "archived", false },
    },
    UserId = "user-id",
});

// Unarchive
var unarchiveResponse = await _channelClient.UnarchiveAsync("messaging", "channel-id", "user-id");
```

## Global Archiving

Channels are archived for a specific member. If the channel should instead be archived for all users, this can be stored as custom data in the channel itself. The value cannot collide with existing fields, so use a value such as `globally_archived: true`.
