The following unread counts are provided by Stream

- A total count of unread messages
- Number of unread channels
- A count of unread threads
- Unread @mentions
- Unread messages per channel
- Unread @mentions per channel
- Unread counts by team
- Unread counts by channel type

Unread counts are first fetched when a user connects.
After that they are updated by events. (new message, mark read, delete message, delete channel etc.)

### Reading Unread Counts

Unread counts are returned when a user connects. After that, you can listen to events to keep them updated in real-time.

```csharp
// Step 1: Get initial unread counts when connecting
var localUserData = await Client.ConnectUserAsync("api_key", "user_id", "user_token");

Debug.Log(localUserData.UnreadChannels);
Debug.Log(localUserData.TotalUnreadCount);

// You can also access unread counts via Client.LocalUserData after connection
// Or subscribe to the Connected event for real-time updates
Client.Connected += (IStreamLocalUserData userData) =>
{
  Debug.Log(userData.UnreadChannels);
  Debug.Log(userData.TotalUnreadCount);
};
```

Note that the higher level SDKs offer convenient endpoints for this. Hooks on react, stateflow on Android etc.
So you only need to use the events manually if you're using plain JS.

### Unread Counts - Server side

The unread endpoint can fetch unread counts server-side, eliminating the need for a client-side connection. It can also be used client-side without requiring a persistent connection to the chat service. This can be useful for including an unread count in notifications or for gently polling when a user loads the application to keep the client up to date without loading up the entire chat.

> [!NOTE]
> A user_id whose unread count is fetched through this method is automatically counted as a Monthly Active User. This may affect your bill.


```csharp
var current = await Client.GetLatestUnreadCountsAsync();

Debug.Log(current.TotalUnreadCount); // Total unread messages
Debug.Log(current.TotalUnreadThreadsCount); // Total unread threads

foreach (var unreadChannel in current.UnreadChannels)
{
  Debug.Log(unreadChannel.ChannelCid); // CID of the channel with unread messages
  Debug.Log(unreadChannel.UnreadCount); // Count of unread messages
  Debug.Log(unreadChannel.LastRead); // Datetime of the last read message
}

foreach (var unreadChannelByType in current.UnreadChannelsByType)
{
  Debug.Log(unreadChannelByType.ChannelType); // Channel type
  Debug.Log(unreadChannelByType.ChannelCount); // How many channels of this type have unread messages
  Debug.Log(unreadChannelByType.UnreadCount); // How many unread messages in all channels of this type
}

foreach (var unreadThread in current.UnreadThreads)
{
  Debug.Log(unreadThread.ParentMessageId); // Message ID of the parent message for this thread
  Debug.Log(unreadThread.LastReadMessageId); // Last read message in this thread
  Debug.Log(unreadThread.UnreadCount); // Count of unread messages
  Debug.Log(unreadThread.LastRead); // Datetime of the last read message
}
```

> [!NOTE]
> This endpoint will return the last 100 unread channels, they are sorted by last_message_at.


#### Batch Fetch Unread

The batch unread endpoint works the same way as the non-batch version with the exception that it can handle multiple user IDs at once and that it is restricted to server-side only.


> [!NOTE]
> If a user ID is not returned in the response then the API couldn't find a user with that ID


### Mark Read

By default the UI component SDKs (React, React Native, ...) mark messages as read automatically when the channel is visible. You can also make the call manually like this:

```csharp
await message.MarkMessageAsLastReadAsync();
```

The `markRead` function can also be executed server-side by passing a user ID as shown in the example below:


It's also possible to mark an already read message as unread:


The mark unread operation can also be executed server-side by passing a user ID:


#### Mark All As Read

You can mark all channels as read for a user like this:

```csharp
// Mark this message as last read
await message.MarkMessageAsLastReadAsync();

// Mark whole channel as read
await channel.MarkChannelReadAsync();
```

## Read State - Showing how far other users have read

When you retrieve a channel from the API (e.g. using query channels), the read state for all members is included in the response. This allows you to display which messages are read by each user. For each member, we include the last time they marked the channel as read.

```csharp
// Every channel maintains a full list of read state for each channel member
foreach (var read in channel.Read)
{
  Debug.Log(read.User); // User
  Debug.Log(read.UnreadMessages); // How many unread messages
  Debug.Log(read.LastRead); // Last read date
}
```

### Unread Messages Per Channel

You can retrieve the count of unread messages for the current user on a channel like this:

```csharp
// Every channel maintains a full list of read state for each channel member
foreach (var read in channel.Read)
{
  Debug.Log(read.User); // User
  Debug.Log(read.UnreadMessages); // How many unread messages
  Debug.Log(read.LastRead); // Last read date
}
```

### Unread Mentions Per Channel

You can retrieve the count of unread messages mentioning the current user on a channel like this:

```csharp
// Will be implemented soon, raise a GitHub issue if you need this feature https://github.com/GetStream/stream-chat-unity/issues/
```
