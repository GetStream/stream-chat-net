using System;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    /// <summary>
    /// Represents a reminder for a message.
    /// </summary>
    public class Reminder
    {
        /// <summary>
        /// The ID of the message this reminder is for.
        /// </summary>
        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        /// <summary>
        /// The ID of the user who owns this reminder.
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// The time when the user should be reminded.
        /// </summary>
        [JsonProperty("remind_at")]
        public DateTime? RemindAt { get; set; }

        /// <summary>
        /// The time when the reminder was created.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The time when the reminder was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Response for reminder operations.
    /// </summary>
    public class ReminderResponse : ApiResponse
    {
        /// <summary>
        /// The reminder that was created, updated, or deleted.
        /// </summary>
        [JsonProperty("reminder")]
        public Reminder Reminder { get; set; }
    }

    /// <summary>
    /// Response for querying reminders.
    /// </summary>
    public class QueryRemindersResponse : ApiResponse
    {
        /// <summary>
        /// The list of reminders that match the query.
        /// </summary>
        [JsonProperty("reminders")]
        public Reminder[] Reminders { get; set; }
    }
}