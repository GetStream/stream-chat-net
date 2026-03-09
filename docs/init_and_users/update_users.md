The Stream user object is central to the chat system and appears in many API responses, effectively following the user throughout the platform. Only an `id` is required to create a user but you can store additional custom data. We recommend only storing what is necessary for Chat such as a username and image URL.

## Client-side User Creation

The `connectUser` method automatically creates _and_ updates the user. If you are looking to onboard your userbase lazily, this is typically a perfectly viable option.

However, it is also common to add your users to Stream before going Live and keep properties of your user base in sync with Stream. For this you'll want to use the `upsertUsers` function server-side and send users in bulk.

## Creating and Updating Users Server-Side

The `upsertUser` method creates or updates a user, replacing its existing data with the new payload (see below for partial updates). To create or update users in batches of up to 100, use the `upsertUsers` or `partialUpdateUsers` APIs, which accept an array of user objects.

Depending on the permission configuration of your application, you may also allow users to update their own user objects client-side.

```csharp
var user = new UserRequest
{
  Id = "bob-1",
  Role = Role.Admin,
};
user.SetData("book", "dune");

await userClient.UpsertManyAsync(new[] { user });
```

And for a batch of users, simply add additional entries (up to 100) into the array you pass to `upsertUsers` :

```csharp
await userClient.UpsertManyAsync(new[] {
 new UserRequest
 {
   Id = userId1,
   Role = Role.Admin,
 },
 new UserRequest
 {
   Id = userId2,
   Role = Role.User,
 },
 new UserRequest
 {
   Id = userId3,
   Role = Role.Admin,
 }
});
```

> [!NOTE]
> If any user in a batch of users contains an error, the entire batch will fail, and the first error encountered will be returned.


## Server-side Partial Updates

If you need to update a subset of properties for a user(s), you can use a partial update method. Both set and unset parameters can be provided to add, modify, or remove attributes to or from the target user(s). The set and unset parameters can be used separately or combined.

```csharp
var updates = new Dictionary<string, object>
{
  { "role", "admin" },
  { "field2.subfield", "test" }
};

var resp = await userClient.UpdatePartialAsync(new UserPartialRequest
{
  Id = user.Id,
  Set = updates,
});
```

> [!NOTE]
> Partial updates support batch requests, similar to the upsertUser endpoint.


## Unique Usernames

Clients can set a username, by setting the `name` custom field. The field is optional and by default has no uniqueness constraints applied to it, however this is configurable by setting the `enforce_unique_username` to either _app_ or _team_.

When checking for uniqueness, the name is _normalized_, by removing any white-space or other special characters, and finally transforming it to lowercase. So "John Doe" is considered a duplicate of "john doe", "john.doe", etc.

With the setting at **app**, creating or updating a user fails if the username already exists anywhere in the app. With **team**, it only fails if the username exists within the same team.

```csharp
// Enable uniqueness constraints on App level
await _appClient.UpdateAppSettingsAsync(new AppSettingsRequest
{
  EnforceUniqueUsernames = UniqueUsernameEnforcementPolicy.App,
});
```

> [!NOTE]
> Enabling this setting will only enforce the constraint going forward and will not try to validate existing usernames.


## Deactivate a User

To deactivate a user, Stream Chat exposes a server-side `deactivateUser` method. A deactivated user cannot connect to Stream Chat but will be present in user queries and channel history.

```csharp
await userClient.DeactivateAsync(userId);

await userClient.DeactivateAsync(userId, true, "joe");
```

## Deactivate Many Users

Many users (up to 100) can be deactivated and reactivated with a single call. The operation runs asynchronously, and the response contains a task_id which can be polled using the [getTask endpoint](/chat/docs/dotnet-csharp#tasks-gettask) to check the status of the operation.

```csharp
// This is a server-side only feature, choose any of our server-side SDKs to use it
```

| Name                  | Type    | Description                                       | Default | Optional |
| --------------------- | ------- | ------------------------------------------------- | ------- | -------- |
| mark_messages_deleted | boolean | Soft deletes all of the messages sent by the user | false   | ✓        |

## Reactivate a User

To reinstate the user as active, use the `reactivateUser` method by passing the users ID as a parameter:

```csharp
await userClient.ReactivateAsync(userId);

await userClient.ReactivateAsync(user2.Id, restoreMessages: true, name: "I am back", createdById: "joe");
```

## Deleting Many Users

You can delete up to 100 users and optionally all of their channels and messages using this method. First the users are marked deleted synchronously so the user will not be directly visible in the API. Then the process deletes the user and related objects asynchronously.

```csharp
var resp = await userClient.DeleteManyAsync(
        new DeleteUsersRequest()
        .WithUserIds(user1.Id, user2.Id)
        .WithUserDeletionStrategy(DeletionStrategy.Hard)
        .WithMessagesDeletionStrategy(DeletionStrategy.Hard)
        .WithConversationsDeletionStrategy(DeletionStrategy.Hard));

var status = await taskClient.GetTaskStatusAsync(resp.TaskId);

var finished = status.Status == AsyncTaskStatus.Completed;
```

The `deleteUsers` method is an asynchronous API where the response contains a task_id which can be polled using the [getTask endpoint](/chat/docs/dotnet-csharp#tasks-gettask) to check the status of the deletions.

These are the request parameters which determine what user data is deleted:

| name                 | type                       | description                                                                                                                                                                                                                                                                               | default | optional |
| -------------------- | -------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------- | -------- |
| user_ids             | array                      | List of users who will be deleted                                                                                                                                                                                                                                                         | -       |          |
| user                 | enum (soft, pruning, hard) | Soft: marks user as deleted and retains all user data. Pruning: marks user as deleted and nullifies user information. Hard: deletes user completely - this requires hard option for **messages** and **conversation** as well.                                                            | -       | ✓        |
| conversations        | enum (soft, hard)          | Soft: marks all conversation channels as deleted (same effect as Delete Channels with 'hard' option disabled). Hard: deletes channel and all its data completely including messages (same effect as Delete Channels with 'hard' option enabled).                                          |         | ✓        |
| messages             | enum (soft, pruning, hard) | Soft: marks all user messages as deleted without removing any related message data. Pruning: marks all user messages as deleted, nullifies message information and removes some message data such as reactions and flags. Hard: deletes messages completely with all related information. | -       | ✓        |
| new_channel_owner_id | string                     | Channels owned by hard-deleted users will be transferred to this userID.                                                                                                                                                                                                                  | -       | ✓        |

> [!NOTE]
> When deleting a user, if you wish to transfer ownership of their channels to another user, provide that user's ID in the `new_channel_owner_id` field. Otherwise, the channel owner will be updated to a system generated ID like `delete-user-8219f6578a7395g`


## Restoring deleted users

If users are _soft_ deleted, they can be restored using the server-side client. However, only the user's metadata is restored; memberships, messages, reactions, etc. are not restored.

You can restore up to 100 users per call:

```csharp
// This is a server-side only feature, choose any of our server-side SDKs to use it
```

## Querying Users

The Query Users method lets you search for users, though in many cases it's more practical to query your own user database instead. Like other Stream query APIs, it accepts filter, sort, and options parameters.

```csharp
var query = QueryUserOptions.Default.WithFilter(new Dictionary<string, object>
{
  { "id", new Dictionary<string, object> { { "$in", new[]{ "john", "jack", "jessie" } } } },
});
query.Limit = 10;
query.Offset = 0;

var resp = await userClient.QueryAsync(query);
```

<partial id="shared/user-management/_query-users-filters-sort-options"></partial>

### Querying with Autocomplete

You can use the `$autocomplete` operator to search for users by name or ID with partial matching.

```csharp
var resp = await _userClient.QueryAsync(QueryUserOptions.Default.WithFilter(new Dictionary<string, object>
{
  { "name", new Dictionary<string, object> { { "$autocomplete", "ro" } } },
}));
```

### Querying Inactive Users

You can use the `last_active` field with the `$exists` operator to find users who have never connected. Use `$exists: false` for users who have never been active, or `$exists: true` for users who have connected at least once.
