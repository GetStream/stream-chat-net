There are two ways to update a channel with the Stream API: partial updates and full updates. A partial update preserves existing custom key–value data, while a full update replaces the entire channel object and removes any fields not included in the request.

## Partial Update

A partial update lets you set or unset specific fields without affecting the rest of the channel’s custom data — essentially a patch-style update.

```csharp
// Create a channel with some custom data
var req = new ChannelGetRequest
{
  Data = new ChannelRequest
  {
    CreatedBy = new UserRequest { Id = createdByUserId },
  },
};

req.Data.SetData("source", "user");
req.Data.SetData("source_detail", new Dictionary<string, object> { { "user_id", 123} });
req.Data.SetData("channel_detail", new Dictionary<string, object>
{
  { "topic", "Plants and Animals"},
  { "rating", "pg" }
});
var channelResp = await channelClient.GetOrCreateAsync("messaging", "general", req);

// Remove channel detail rating, and set a new source
var req = new PartialUpdateChannelRequest
{

  Unset = new List<string> { "channel_detail.rating" },
  Set = new Dictionary<string, object> { { "source", "system" } }
};
await channelClient.PartialUpdateAsync(channelResp.Channel.Type, channelResp.Channel.Id, req);
```

## Full Update

The `update` function updates all of the channel data. **Any data that is present on the channel and not included in a full update will be deleted.**

```csharp
var updatedChannel = new ChannelUpdateRequest { Data = new ChannelRequest() };
updatedChannel.Data.SetData("name", "myspecialchannel");
updatedChannel.Data.SetData("color", "green");

await channelClient.UpdateAsync(channel.Type, channel.Id, updatedChannel);
```

### Request Params

| Name         | Type   | Description                                                                                                                                                                          | Optional |
| ------------ | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | -------- |
| channel data | object | Object with the new channel information. One special field is "frozen". Setting this field to true will freeze the channel. Read more about freezing channels in "Freezing Channels" |          |
| text         | object | Message object allowing you to show a system message in the Channel that something changed.                                                                                          | Yes      |

> [!NOTE]
> Updating a channel using these methods cannot be used to add or remove members. For this, you must use specific methods for adding/removing members, more information can be found [here](/chat/docs/dotnet-csharp/channel_members/).
