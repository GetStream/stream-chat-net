Scheduled messages let a user write a message now and have it sent to a channel later. Server-side, you can schedule, edit, cancel and list scheduled messages on behalf of any user.

Scheduled messages must be enabled for your app. Until the message is sent, only the user who scheduled it can see it; nothing is sent to the channel.

## Scheduling a Message

Send a message with a `scheduled_at` time. It must be at least 1 minute and at most the app's maximum lead time (120 days by default) from now. Slash commands and pending messages cannot be scheduled.

```csharp
var resp = await messageClient.ScheduleMessageAsync("messaging", "general",
    new MessageRequest { Text = "Good morning!" }, "user-id", DateTimeOffset.UtcNow.AddHours(1));

var scheduledMessageId = resp.ScheduledMessage.Id;
```

`SendMessageAsync` with `SendMessageOptions.ScheduledAt` does the same. The response carries `ScheduledMessage`. Its `Id` is used to edit, cancel or send the message now. The `Message` in the response holds the content as it will be sent; it is not in the channel yet.

## Updating a Scheduled Message

Change the message, the send time, or both. A new message replaces the scheduled one as a whole; the sender stays the user it was scheduled for.

```csharp
await messageClient.UpdateScheduledMessageAsync(scheduledMessageId, new UpdateScheduledMessageRequest
{
    Message = new MessageRequest { Text = "Good afternoon!" },
    ScheduledAt = DateTimeOffset.UtcNow.AddHours(2),
});
```

## Sending a Scheduled Message Now

```csharp
await messageClient.SendScheduledMessageNowAsync(scheduledMessageId);
```

## Deleting a Scheduled Message

Deleting cancels a scheduled message so it is never sent, or removes a failed one.

```csharp
await messageClient.DeleteScheduledMessageAsync(scheduledMessageId);
```

Updating, deleting or sending now fails with a 409 error while the message is being sent.

## Querying Scheduled Messages

List the scheduled messages of a user across channels: pending ones first, then failed ones, each by `scheduled_at`. You can filter by channel and status (`scheduled`, `sending`, `failed`).

```csharp
var page = await messageClient.QueryScheduledMessagesAsync(new QueryScheduledMessagesRequest
{
    UserId = "user-id",
    Filter = new ScheduledMessagesFilter
    {
        Cid = "messaging:general",
        Status = new List<string> { ScheduledMessageStatus.Failed },
    },
    Limit = 20,
});

// next page
if (!string.IsNullOrEmpty(page.Next))
{
    page = await messageClient.QueryScheduledMessagesAsync(new QueryScheduledMessagesRequest
    {
        UserId = "user-id",
        Limit = 20,
        Next = page.Next,
    });
}
```

## Failed Messages

When a scheduled message cannot be sent, its `Status` becomes `failed` and `FailureReason` says why (see `ScheduledMessageFailureReason`): `banned`, `no_permission`, `channel_frozen`, `channel_deleted`, `user_deleted`, `moderation_blocked`, `hook_rejected`, `expired`, `internal_error`, `invalid_message` or `pending_not_supported`. A failed message is not retried; delete it, and schedule it again if needed.

## Webhooks

The `scheduled_message.sent` and `scheduled_message.failed` events are sent to webhooks. `EventResponse.ScheduledMessage` holds the scheduled message, and on `scheduled_message.sent`, `GetData<string>("message_id")` returns the ID of the sent message.
