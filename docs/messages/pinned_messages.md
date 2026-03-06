Pinned messages highlight important content in a channel. Use them for announcements, key information, or temporarily promoted content. Each channel can have multiple pinned messages, with optional expiration times.

## Pinning and Unpinning Messages

Pin an existing message using `pinMessage`, or create a pinned message by setting `pinned: true` when sending.

```csharp
var msg = new MessageRequest { Text = "Important announcement", Pinned = true };
var response = await messageClient.SendMessageAsync(channel.Type, channel.Id, msg, user.Id);

// Pin message for 120 seconds
await messageClient.PinMessageAsync(response.Message.Id, user.Id, TimeSpan.FromSeconds(120));

// Unpin message
await messageClient.UnpinMessageAsync(response.Message.Id, user.Id);
```

### Pin Parameters

| Name        | Type    | Description                                                            | Default | Optional |
| ----------- | ------- | ---------------------------------------------------------------------- | ------- | -------- |
| pinned      | boolean | Whether the message is pinned                                          | false   | ✓        |
| pinned_at   | string  | Timestamp when the message was pinned                                  | -       | ✓        |
| pin_expires | string  | Timestamp when the pin expires. Null means the message does not expire | null    | ✓        |
| pinned_by   | object  | The user who pinned the message                                        | -       | ✓        |

> [!NOTE]
> Pinning a message requires the `PinMessage` permission. See [Permission Resources](/chat/docs/dotnet-csharp/permissions_reference/) and [Default Permissions](/chat/docs/dotnet-csharp/chat_permission_policies/) for details.


## Retrieving Pinned Messages

Query a channel to retrieve the 10 most recent pinned messages from `pinned_messages`.

```csharp
var currentChannelState = await _channelClient.GetOrCreateAsync(_channel.Type, _channel.Id, new ChannelGetRequest
{
  State = true,
  Watch = false,
});

var pinnedMessages = currentChannelState.PinnedMessages;
```

## Paginating Pinned Messages

Use the dedicated pinned messages endpoint to retrieve all pinned messages with pagination.
