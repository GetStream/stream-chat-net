Muting a channel prevents it from triggering push notifications, unhiding, or incrementing the unread count for that user.

By default, mutes remain active indefinitely until removed. You can optionally set an expiration time. The list of muted channels and their expiration times is returned when the user connects.

## Mute a Channel

```csharp
await channelClient.MuteChannelAsync(new ChannelMuteRequest
{
  ChannelCids = new[] { "<channel-cid-here>" },
  UserId = "john",
});

// With expiration
await channelClient.MuteChannelAsync(new ChannelMuteRequest
{
  ChannelCids = new[] { "<channel-cid-here>" },
  UserId = "john",
  Expiration = 1000,
});
```

> [!NOTE]
> Messages added to muted channels do not increase the unread messages count.


### Query Muted Channels

Muted channels can be filtered or excluded by using the `muted` in your query channels filter.

```csharp
await channelClient.QueryChannelsAsync(QueryChannelsOptions.Default.WithFilter(new Dictionary<string, object>
{
  { "muted", true },
}));
```

### Remove a Channel Mute

Use the unmute method to restore normal notifications and unread behavior for a channel.

```csharp
await channelClient.UnmuteChannelAsync(new ChannelUnmuteRequest
{
  ChannelCids = new[] { "<channel-cid-here>" },
  UserId = "john",
});
```
