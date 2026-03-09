Disabling a channel is a visibility and access toggle. The channel and all its data remain intact, but client-side read and write operations return a `403 Not Allowed` error. Server-side access is preserved for admin operations like moderation and data export.

Disabled channels still appear in query results by default. This means users see the channel in their list but receive errors when attempting to open it. To hide disabled channels from users, filter them out in your queries:


Re-enabling a channel restores full client-side access with all historical messages intact.

## Disable a Channel

```csharp
// disable a channel with full update
var updateReq = new ChannelUpdateRequest { Data = new ChannelRequest() };
updateReq.Data.SetData("disabled", true);
await channelClient.UpdateAsync("<channel-type>", "<channel-id>", updateReq);

// disable a channel with partial update
var channelUpdates = new PartialUpdateChannelRequest
{
  Set = new Dictionary<string, object> { { "disabled", true } },
};
await channelClient.PartialUpdateAsync("<channel-type>", "<channel-id>", channelUpdates);

// enable a channel with full update
var updateReq = new ChannelUpdateRequest { Data = new ChannelRequest() };
updateReq.Data.SetData("disabled", false);
await channelClient.UpdateAsync("<channel-type>", "<channel-id>", updateReq);

// enable a channel with partial update
var enableUpdates = new PartialUpdateChannelRequest
{
  Set = new Dictionary<string, object> { { "disabled", false } },
};
await channelClient.PartialUpdateAsync("<channel-type>", "<channel-id>", enableUpdates);
```

> [!NOTE]
> To prevent new messages while still allowing users to read existing messages, use [freeze the channel](/chat/docs/dotnet-csharp/freezing_channels/) instead.
