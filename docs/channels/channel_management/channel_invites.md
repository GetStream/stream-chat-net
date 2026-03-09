Invites allow you to add users to a channel with a pending state. The invited user receives a notification and can accept or reject the invite.

Unread counts are not incremented for channels with a pending invite.

## Invite Users

```csharp
await channelClient.InviteAsync("<channel-type>", "<channel-id>", "thierry");
```

## Accept an Invite

Call `acceptInvite` to accept a pending invite. You can optionally include a `message` parameter to post a system message when the user joins (e.g., "Nick joined this channel!").

```csharp
await channelClient.AcceptInviteAsync("<channel-type>", "<channel-id>", "thierry");
```

## Reject an Invite

Call `rejectInvite` to decline a pending invite. Client-side calls use the currently connected user. Server-side calls require a `user_id` parameter.

```csharp
await channelClient.RejectInviteAsync("<channel-type>", "<channel-id>", "thierry");
```

## Query Invites by Status

Use `queryChannels` with the `invite` filter to retrieve channels based on invite status. Valid values are `pending`, `accepted`, and `rejected`.

### Query Accepted Invites

```csharp
await channelClient.QueryChannelsAsync(QueryChannelsOptions.Default.WithFilter(new Dictionary<string, object>
{
  { "invite", "accepted" },
}));
```

### Query Rejected Invites

```csharp
await channelClient.QueryChannelsAsync(QueryChannelsOptions.Default.WithFilter(new Dictionary<string, object>
{
  { "invite", "rejected" },
}));
```

### Query Pending Invites

```csharp
await channelClient.QueryChannelsAsync(QueryChannelsOptions.Default.WithFilter(new Dictionary<string, object>
{
  { "invite", "pending" },
}));
```
