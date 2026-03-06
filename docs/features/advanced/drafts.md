Draft messages allow users to save messages as drafts for later use. This feature is useful when users want to compose a message but aren't ready to send it yet.

## Creating a draft message

It is possible to create a draft message for a channel or a thread. Only one draft per channel/thread can exist at a time, so a newly created draft overrides the existing one.


## Deleting a draft message

You can delete a draft message for a channel or a thread as well.


## Loading a draft message

It is also possible to load a draft message for a channel or a thread. Although, when querying channels, each channel will contain the draft message payload, in case there is one. The same for threads (parent messages). So, for the most part this function will not be needed.


## Querying draft messages

The Stream Chat SDK provides a way to fetch all the draft messages for the current user. This can be useful to for the current user to manage all the drafts they have in one place.


Filtering is possible on the following fields:

| Name        | Type                       | Description                    | Supported operations      | Example                                                |
| ----------- | -------------------------- | ------------------------------ | ------------------------- | ------------------------------------------------------ |
| channel_cid | string                     | the ID of the message          | $in, $eq                  | { channel_cid: { $in: [ 'channel-1', 'channel-2' ] } } |
| parent_id   | string                     | the ID of the parent message   | $in, $eq, $exists         | { parent_id: 'parent-message-id' }                     |
| created_at  | string (RFC3339 timestamp) | the time the draft was created | $eq, $gt, $lt, $gte, $lte | { created_at: { $gt: '2024-04-24T15:50:00.00Z' }       |

Sorting is possible on the `created_at` field. By default, draft messages are returned with the newest first.

### Pagination

In case the user has a lot of draft messages, you can paginate the results.


## Events

The following WebSocket events are available for draft messages:

- `draft.updated`, triggered when a draft message is updated.
- `draft.deleted`, triggered when a draft message is deleted.

You can subscribe to these events using the Stream Chat SDK.
