User presence allows you to show when a user was last active and if they are online right now. This feature can be enabled or disabled per channel type in the [channel type settings](/chat/docs/dotnet-csharp/channel_features/).

## Listening to Presence Changes

To receive presence updates, you need to watch a channel or query channels with `presence: true`. This allows you to show a user as offline when they leave and update their status in real time.

```csharp
// Get a channel
var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

// Members are users belonging to this channel
var member = channel.Members.First();

// Each member object contains a user object. A single user can be a member of many channels
var user = member.User;

// Each user object exposes the PresenceChange event that will trigger when Online status changes
user.PresenceChanged += (userObj, isOnline, isActive) =>
{

};
```

A users online status change can be handled via event delegation by subscribing to the `user.presence.changed` event the same you do for any other event.

## Presence Data Format

Whenever you read a user the presence data will look like this:

```csharp
// Get a channel
var channel = await Client.GetOrCreateChannelWithIdAsync(ChannelType.Messaging, "my-channel-id");

// Members are users belonging to this channel
var member = channel.Members.First();

// Each member object contains a user object. A single user can be a member of many channels
var user = member.User;

var message = channel.Messages.First();

// Each message contains the author user object
var user2 = message.User;

// Presence related fields on a user object
var isOnline = user.Online;
var lastActive = user.LastActive;
```

> [!NOTE]
> The online field indicates if the user is online. The status field stores text indicating the current user status.


> [!NOTE]
> The last_active field is updated when a user connects and then refreshed every 15 minutes.


## Invisible

To mark your user as invisible, you can update your user to set the invisible property to _true_. Your user will remain invisible even if you disconnect and reconnect. You must explicitly set invisible to _false_ in order to become visible again.

```csharp
// Get local user object
var localUserData = await Client.ConnectUserAsync("api-key", "user-id", "user-token");

// Or like this
var localUserData2 = Client.LocalUserData;

// Get local user object
var localUser = localUserData.User;

// Check local user invisibility status
var isInvisible = localUser.Invisible;

// Mark invisible
await localUser.MarkInvisibleAsync();

// Mark visible
await localUser.MarkVisibleAsync();
```

You can also set your user to invisible when connecting by setting the invisible property to _true_. You can also set a custom status message at the same time:

```csharp
// Will be implemented soon, please send a support ticket if you need this feature
```

> [!NOTE]
> When invisible is set to _true,_ the current user will appear as offline to other users.
