Message reminders let users schedule notifications for specific messages, making it easier to follow up later. When a reminder includes a timestamp, it's like saying "remind me later about this message," and the user who set it will receive a notification at the designated time. If no timestamp is provided, the reminder functions more like a bookmark, allowing the user to save the message for later reference.

Reminders require Push V3 to be enabled - see details [here](/chat/docs/dotnet-csharp/push_template/)

## Enabling Reminders

The Message Reminders feature must be activated at the channel level before it can be used. You have two configuration options: activate it for a single channel using configuration overrides, or enable it globally for all channels of a particular type.

```csharp
// Enabling it for a channel
await channelClient.PartialUpdateAsync(channel.Type, channel.Id, new PartialUpdateChannelRequest
{
    Set = new Dictionary<string, object>
    {
        { "config_overrides", new Dictionary<string, object> { { "user_message_reminders", true } } }
    }
});

// Enabling it for a channel type
await channelTypeClient.UpdateChannelTypeAsync("messaging", new ChannelTypeWithStringCommandsRequest
{
    UserMessageReminders = true
});
```

Message reminders allow users to:

- schedule a notification after given amount of time has elapsed
- bookmark a message without specifying a deadline

## Limits

- A user cannot have more than 250 reminders scheduled
- A user can only have one reminder created per message

## Creating a Message Reminder

You can create a reminder for any message. When creating a reminder, you can specify a reminder time or save it for later without a specific time.

```csharp
// Create a reminder with a specific due date
var reminder = await messageClient.CreateReminderAsync("message-id", "user-id", DateTimeOffset.Now.AddHours(1));

// Create a "Save for later" reminder without a specific time
var reminder = await messageClient.CreateReminderAsync("message-id", "user-id");
```

## Updating a Message Reminder

You can update an existing reminder for a message to change the reminder time.

```csharp
// Update a reminder with a new due date
var updatedReminder = await messageClient.UpdateReminderAsync("message-id", "user-id", DateTimeOffset.Now.AddHours(2));

// Convert a timed reminder to "Save for later"
var updatedReminder = await messageClient.UpdateReminderAsync("message-id", "user-id", null);
```

## Deleting a Message Reminder

You can delete a reminder for a message when it's no longer needed.

```csharp
// Delete the reminder for the message
await messageClient.DeleteReminderAsync("message-id", "user-id");
```

## Querying Message Reminders

The SDK allows you to fetch all reminders of the current user. You can filter, sort, and paginate through all the user's reminders.

```csharp
// Query reminders for a user
var reminders = await messageClient.QueryRemindersAsync("user-id");

// Query reminders with filters
var filter = new Dictionary<string, object> { { "channel_cid", "messaging:general" } };
var reminders = await messageClient.QueryRemindersAsync("user-id", filter);
```

### Filtering Reminders

You can filter the reminders based on different criteria:

- `message_id` - Filter by the message that the reminder is created on.
- `remind_at` - Filter by the reminder time.
- `created_at` - Filter by the creation date.
- `channel_cid` - Filter by the channel ID.

The most common use case would be to filter by the reminder time. Like filtering overdue reminders, upcoming reminders, or reminders with no due date (saved for later).

```csharp
// Filter overdue reminders
var overdueFilter = new Dictionary<string, object>
{
    { "remind_at", new Dictionary<string, object> { { "$lt", DateTimeOffset.Now } } }
};
var overdueReminders = await messageClient.QueryRemindersAsync("user-id", overdueFilter);

// Filter upcoming reminders
var upcomingFilter = new Dictionary<string, object>
{
    { "remind_at", new Dictionary<string, object> { { "$gt", DateTimeOffset.Now } } }
};
var upcomingReminders = await messageClient.QueryRemindersAsync("user-id", upcomingFilter);

// Filter reminders with no due date (saved for later)
var savedFilter = new Dictionary<string, object> { { "remind_at", null } };
var savedReminders = await messageClient.QueryRemindersAsync("user-id", savedFilter);
```

### Pagination

If you have many reminders, you can paginate the results.

```csharp
// Load reminders with pagination
var options = new Dictionary<string, object> { { "limit", 10 }, { "offset", 0 } };
var reminders = await messageClient.QueryRemindersAsync("user-id", null, options);

// Load next page
var nextPageOptions = new Dictionary<string, object> { { "limit", 10 }, { "offset", 10 } };
var nextReminders = await messageClient.QueryRemindersAsync("user-id", null, nextPageOptions);
```

## Events

The following WebSocket events are available for message reminders:

- `reminder.created` - Triggered when a reminder is created
- `reminder.updated` - Triggered when a reminder is updated
- `reminder.deleted` - Triggered when a reminder is deleted
- `notification.reminder_due` - Triggered when a reminder's due time is reached

When a reminder's due time is reached, the server also sends a push notification to the user. Ensure push notifications are configured in your app.


## Webhooks

The same events are available as webhooks to notify your backend systems:

- `reminder.created`
- `reminder.updated`
- `reminder.deleted`
- `notification.reminder_due`

These webhook events contain the same payload structure as their WebSocket counterparts. For more information on configuring webhooks, see the [Webhooks documentation](/chat/docs/dotnet-csharp/webhook_events/).
