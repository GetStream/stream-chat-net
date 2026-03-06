Truncating a channel removes all messages but preserves the channel data and members. To delete both the channel and its messages, use [Delete Channel](/chat/docs/dotnet-csharp/channel_delete/) instead.

Truncation can be performed client-side or server-side. Client-side truncation requires the `TruncateChannel` permission.

On server-side calls, use the `user_id` field to identify who performed the truncation.

By default, truncation hides messages. To permanently delete messages, set `hard_delete` to `true`.

## Truncate a Channel

```csharp
await channelClient.TruncateAsync(channel.Type, channel.Id);

// Or with parameters:
await channelClient.TruncateAsync(channel.Type, channel.Id, new TruncateOptions
{
  SkipPush = true,
  HardDelete = true,
  Message = new MessageRequest
  {
    Text = "Dear Everyone. The channel has been truncated.",
    User = new UserRequest { Id = user.Id },
  },
});
```

## Truncate Options

| Field        | Type   | Description                                            | Optional |
| ------------ | ------ | ------------------------------------------------------ | -------- |
| truncated_at | Date   | Truncate messages up to this time                      | ✓        |
| user_id      | string | User who performed the truncation (server-side only)   | ✓        |
| message      | object | A system message to add after truncation               | ✓        |
| skip_push    | bool   | Do not send a push notification for the system message | ✓        |
| hard_delete  | bool   | Permanently delete messages instead of hiding them     | ✓        |
