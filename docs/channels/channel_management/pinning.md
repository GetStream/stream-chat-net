Channel members can pin a channel for themselves. This is a per-user setting that does not affect other members.

Pinned channels function identically to regular channels via the API, but your UI can display them separately. When a channel is pinned, the timestamp is recorded and returned as `pinned_at` in the response.

When querying channels, filter by `pinned: true` to retrieve only pinned channels, or `pinned: false` to exclude them. You can also sort by `pinned_at` to display pinned channels first.

## Pin a Channel

```csharp
// Pin
var pinResponse = await _channelClient.PinAsync("messaging", "channel-id", "user-id");

// Get the date when the channel got pinned by the user
var pinnedAt = pinResponse.ChannelMember.PinnedAt;

// Get channels pinned for the user
var pinnedChannels = await _channelClient.QueryChannelsAsync(new QueryChannelsOptions
{
    Filter = new Dictionary<string, object>()
    {
        { "pinned", true },
    },
    UserId = "user-id",
});

// Unpin
var unpinResponse = await _channelClient.UnpinAsync("messaging", "channel-id", "user-id");
```

## Global Pinning

Channels are pinned for a specific member. If the channel should instead be pinned for all users, this can be stored as custom data in the channel itself. The value cannot collide with existing fields, so use a value such as `globally_pinned: true`.
