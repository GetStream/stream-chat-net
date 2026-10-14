using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    /// <summary>
    /// Statuses of a scheduled message.
    /// </summary>
    public static class ScheduledMessageStatus
    {
        /// <summary>Waiting for its send time.</summary>
        public const string Scheduled = "scheduled";

        /// <summary>Being sent. It cannot be updated, deleted or sent now in this state.</summary>
        public const string Sending = "sending";

        /// <summary>Sending failed; <see cref="ScheduledMessage.FailureReason"/> says why.</summary>
        public const string Failed = "failed";

        /// <summary>Deleted before it was sent.</summary>
        public const string Canceled = "canceled";

        /// <summary>Sent. Only reported by the scheduled_message.sent event.</summary>
        public const string Sent = "sent";
    }

    /// <summary>
    /// Reasons a scheduled message failed to send.
    /// </summary>
    public static class ScheduledMessageFailureReason
    {
        public const string Banned = "banned";
        public const string NoPermission = "no_permission";
        public const string ChannelFrozen = "channel_frozen";
        public const string ChannelDeleted = "channel_deleted";
        public const string UserDeleted = "user_deleted";
        public const string ModerationBlocked = "moderation_blocked";
        public const string HookRejected = "hook_rejected";
        public const string Expired = "expired";
        public const string InternalError = "internal_error";
        public const string InvalidMessage = "invalid_message";
        public const string PendingNotSupported = "pending_not_supported";
    }

    /// <summary>
    /// A message scheduled to be sent later.
    /// </summary>
    public class ScheduledMessage
    {
        /// <summary>
        /// The ID of the scheduled message, used to update, delete or send it now.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The CID of the channel the message will be sent to.
        /// </summary>
        [JsonProperty("cid")]
        public string Cid { get; set; }

        /// <summary>
        /// The user who scheduled the message, and who will send it.
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }

        /// <summary>
        /// The message that will be sent.
        /// </summary>
        [JsonProperty("message")]
        public ScheduledMessageContent Message { get; set; }

        /// <summary>
        /// When the message is scheduled to be sent.
        /// </summary>
        [JsonProperty("scheduled_at")]
        public DateTimeOffset? ScheduledAt { get; set; }

        /// <summary>
        /// The status of the scheduled message. See <see cref="ScheduledMessageStatus"/>.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Why sending failed. Only set when <see cref="Status"/> is failed. See <see cref="ScheduledMessageFailureReason"/>.
        /// </summary>
        [JsonProperty("failure_reason")]
        public string FailureReason { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// The channel of the scheduled message. Only set in query results.
        /// </summary>
        [JsonProperty("channel")]
        public ScheduledMessageChannel Channel { get; set; }
    }

    /// <summary>
    /// The message a scheduled message will send. Custom fields are available
    /// through <see cref="CustomDataBase.GetData{T}(string)"/>.
    /// </summary>
    public class ScheduledMessageContent : CustomDataBase
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("mml")]
        public string Mml { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty("parent_id")]
        public string ParentId { get; set; }

        [JsonProperty("show_in_channel")]
        public bool? ShowInChannel { get; set; }

        [JsonProperty("quoted_message_id")]
        public string QuotedMessageId { get; set; }

        /// <summary>
        /// The IDs of the mentioned users.
        /// </summary>
        [JsonProperty("mentioned_users")]
        public List<string> MentionedUsers { get; set; }

        [JsonProperty("mentioned_roles")]
        public List<string> MentionedRoles { get; set; }

        [JsonProperty("mentioned_group_ids")]
        public List<string> MentionedGroupIds { get; set; }

        [JsonProperty("mentioned_channel")]
        public bool? MentionedChannel { get; set; }

        [JsonProperty("mentioned_here")]
        public bool? MentionedHere { get; set; }

        [JsonProperty("restricted_visibility")]
        public List<string> RestrictedVisibility { get; set; }

        [JsonProperty("silent")]
        public bool? Silent { get; set; }

        [JsonProperty("pinned")]
        public bool? Pinned { get; set; }

        [JsonProperty("pin_expires")]
        public DateTimeOffset? PinExpires { get; set; }

        [JsonProperty("poll_id")]
        public string PollId { get; set; }

        [JsonProperty("shared_location")]
        public ScheduledMessageLocation SharedLocation { get; set; }
    }

    /// <summary>
    /// The location a scheduled message will share.
    /// </summary>
    public class ScheduledMessageLocation
    {
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }

        [JsonProperty("longitude")]
        public double? Longitude { get; set; }

        [JsonProperty("created_by_device_id")]
        public string CreatedByDeviceId { get; set; }

        [JsonProperty("end_at")]
        public DateTimeOffset? EndAt { get; set; }
    }

    /// <summary>
    /// The channel of a scheduled message.
    /// </summary>
    public class ScheduledMessageChannel
    {
        [JsonProperty("cid")]
        public string Cid { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
    }

    /// <summary>
    /// Changes to a scheduled message. At least one of <see cref="Message"/> or
    /// <see cref="ScheduledAt"/> is required.
    /// </summary>
    public class UpdateScheduledMessageRequest
    {
        /// <summary>
        /// Replaces the scheduled message as a whole. The sender stays the user the message was scheduled for.
        /// </summary>
        [JsonProperty("message")]
        public MessageRequest Message { get; set; }

        /// <summary>
        /// The new send time. Must be at least 1 minute and at most the app's maximum lead time from now.
        /// </summary>
        [JsonProperty("scheduled_at")]
        public DateTimeOffset? ScheduledAt { get; set; }
    }

    /// <summary>
    /// Narrows a scheduled messages query. Empty fields mean no restriction.
    /// </summary>
    public class ScheduledMessagesFilter
    {
        /// <summary>
        /// Only scheduled messages of this channel.
        /// </summary>
        [JsonProperty("cid")]
        public string Cid { get; set; }

        /// <summary>
        /// Only these statuses: scheduled, sending and/or failed.
        /// </summary>
        [JsonProperty("status")]
        public List<string> Status { get; set; }
    }

    /// <summary>
    /// Lists the scheduled messages of one user across channels.
    /// </summary>
    public class QueryScheduledMessagesRequest
    {
        /// <summary>
        /// The user whose scheduled messages to list. Required.
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("filter")]
        public ScheduledMessagesFilter Filter { get; set; }

        /// <summary>
        /// Only scheduled_at ascending is supported.
        /// </summary>
        [JsonProperty("sort")]
        public List<SortParameter> Sort { get; set; }

        /// <summary>
        /// Page size, 1 to 100. Defaults to 20.
        /// </summary>
        [JsonProperty("limit")]
        public int? Limit { get; set; }

        /// <summary>
        /// Cursor from the previous page.
        /// </summary>
        [JsonProperty("next")]
        public string Next { get; set; }
    }

    /// <summary>
    /// Response of the scheduled message update, delete and send now operations.
    /// </summary>
    public class ScheduledMessageResponse : ApiResponse
    {
        [JsonProperty("scheduled_message")]
        public ScheduledMessage ScheduledMessage { get; set; }
    }

    /// <summary>
    /// One page of scheduled messages: pending ones first, then failed ones, each by scheduled_at.
    /// </summary>
    public class QueryScheduledMessagesResponse : ApiResponse
    {
        [JsonProperty("scheduled_messages")]
        public List<ScheduledMessage> ScheduledMessages { get; set; }

        /// <summary>
        /// Cursor for the next page; empty on the last page.
        /// </summary>
        [JsonProperty("next")]
        public string Next { get; set; }
    }
}
