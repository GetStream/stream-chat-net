Location sharing allows users to send a static position or share their real-time location with other participants in a channel. Stream Chat supports both static and live location sharing.

There are two types of location sharing:

- **Static Location**: A one-time location share that does not update over time.
- **Live Location**: A real-time location sharing that updates over time.

> [!NOTE]
> The SDK handles location message creation and updates, but location tracking must be implemented by the application using device location services.


## Enabling location sharing

The location sharing feature must be activated at the channel level before it can be used. You have two configuration options: activate it for a single channel using configuration overrides, or enable it globally for all channels of a particular type via [channel type settings](/chat/docs/dotnet-csharp/channel_features/).

```csharp
// Enabling it for a channel type
await channelTypeClient.UpdateChannelTypeAsync("messaging", new ChannelTypeWithStringCommandsRequest
{
  SharedLocations = true
});
```

## Sending static location

Static location sharing allows you to send a message containing a static location.

```csharp
var channel = await CreateChannelAsync(createdByUserId: _user1.Id, members: new[] { _user1.Id });

// Create a shared location for the initial message
var location = new SharedLocationRequest
{
    Longitude = longitude,
    Latitude = latitude,
    EndAt = null, // null for static location
    CreatedByDeviceId = "test-device",
};

// Send a message with shared location
var messageRequest = new MessageRequest
{
    Text = "Test message for shared location",
    SharedLocation = location,
};
var messageResp = await _messageClient.SendMessageAsync(
    channel.Type,
    channel.Id,
    messageRequest,
    _user1.Id);
```

## Starting live location sharing

Live location sharing enables real-time location updates for a specified duration. The SDK manages the location message lifecycle, but your application is responsible for providing location updates.

```csharp
var channel = await CreateChannelAsync(createdByUserId: _user1.Id, members: new[] { _user1.Id });

// Create a shared location for live location sharing (with EndAt)
var location = new SharedLocationRequest
{
    Longitude = longitude,
    Latitude = latitude,
    EndAt = DateTimeOffset.UtcNow.AddHours(1), // Set duration for live location
    CreatedByDeviceId = "test-device",
};

// Send a message with shared location
var messageRequest = new MessageRequest
{
    Text = "Test message for live location sharing",
    SharedLocation = location,
};
var messageResp = await _messageClient.SendMessageAsync(
    channel.Type,
    channel.Id,
    messageRequest,
    _user1.Id);
```

## Stopping live location sharing

You can stop live location sharing for a specific message using the message controller:

```csharp
// Stop live location sharing by setting EndAt to now
var stopLocationRequest = new SharedLocationRequest
{
    MessageId = liveLocationMessageId, // The ID of the live location message
    Longitude = longitude,
    Latitude = latitude,
    EndAt = DateTimeOffset.UtcNow, // Set EndAt to now to stop sharing
    CreatedByDeviceId = "test-device",
};

// Update the live location
var response = await UpdateLocationAsync(userID, stopLocationRequest);
```

## Updating live location

Your application must implement location tracking and provide updates to the SDK. The SDK handles updating all the current user's active live location messages and provides a throttling mechanism to prevent excessive API calls.

```csharp
// Get all active live locations for the current user
var activeLocations = await GetSharedLocationsAsync(userID);

// New location to set (e.g., from device location update)
var newLocation = new SharedLocationRequest
{
    Longitude = newLongitude, // updated longitude
    Latitude = newLatitude,   // updated latitude
    EndAt = endAt,            // keep the same end time
    CreatedByDeviceId = "test-device",
};

// Update all active live locations for the user
foreach (var location in activeLocations.ActiveLiveLocations)
{
    newLocation.MessageId = location.MessageId;
    await UpdateLocationAsync(userID, newLocation);
}
```

Whenever the location is updated, the message will automatically be updated with the new location.

The SDK will also notify your application when it should start or stop location tracking as well as when the active live location messages change.


## Events

Whenever a location is created or updated, the following WebSocket events will be sent:

- `message.new`: When a new location message is created.
- `message.updated`: When a location message is updated.

> [!NOTE]
> In Dart, these events are resolved to more specific location events:
>
> - `location.shared`: When a new location message is created.
> - `location.updated`: When a location message is updated.


You can easily check if a message is a location message by checking the `message.sharedLocation` property. For example, you can use this events to render the locations in a map view.
