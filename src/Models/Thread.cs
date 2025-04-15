using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StreamChat.Models
{
    public class Thread : CustomDataBase
    {
        [JsonProperty("app_pk")]
        public int AppPK { get; set; }

        [JsonProperty("channel_cid")]
        public string ChannelCID { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("parent_message_id")]
        public string ParentMessageID { get; set; }

        [JsonProperty("parent_message")]
        public Message ParentMessage { get; set; }

        [JsonProperty("created_by_user_id")]
        public string CreatedByUserID { get; set; }

        [JsonProperty("created_by")]
        public User CreatedBy { get; set; }

        [JsonProperty("reply_count")]
        public int ReplyCount { get; set; }

        [JsonProperty("participant_count")]
        public int ParticipantCount { get; set; }

        [JsonProperty("active_participant_count")]
        public int ActiveParticipantCount { get; set; }

        [JsonProperty("participants")]
        public List<ThreadParticipant> Participants { get; set; }

        [JsonProperty("last_message_at")]
        public DateTimeOffset? LastMessageAt { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty("deleted_at")]
        public DateTimeOffset? DeletedAt { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public class ThreadParticipant : CustomDataBase
    {
        [JsonProperty("app_pk")]
        public int AppPK { get; set; }

        [JsonProperty("channel_cid")]
        public string ChannelCID { get; set; }

        [JsonProperty("last_thread_message_at")]
        public DateTimeOffset? LastThreadMessageAt { get; set; }

        [JsonProperty("thread_id")]
        public string ThreadID { get; set; }

        [JsonProperty("user_id")]
        public string UserID { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("left_thread_at")]
        public DateTimeOffset? LeftThreadAt { get; set; }

        [JsonProperty("last_read_at")]
        public DateTimeOffset LastReadAt { get; set; }
    }

    public class ThreadResponse : CustomDataBase
    {
        [JsonProperty("channel_cid")]
        public string ChannelCID { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("parent_message_id")]
        public string ParentMessageID { get; set; }

        [JsonProperty("parent_message")]
        public Message ParentMessage { get; set; }

        [JsonProperty("created_by_user_id")]
        public string CreatedByUserID { get; set; }

        [JsonProperty("created_by")]
        public User CreatedBy { get; set; }

        [JsonProperty("reply_count")]
        public int ReplyCount { get; set; }

        [JsonProperty("participant_count")]
        public int ParticipantCount { get; set; }

        [JsonProperty("active_participant_count")]
        public int ActiveParticipantCount { get; set; }

        [JsonProperty("participants")]
        public List<ThreadParticipant> Participants { get; set; }

        [JsonProperty("last_message_at")]
        public DateTimeOffset? LastMessageAt { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [JsonProperty("deleted_at")]
        public DateTimeOffset? DeletedAt { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("latest_replies")]
        public List<Message> LatestReplies { get; set; }

        [JsonProperty("read")]
        public ChannelRead Read { get; set; }
    }

    public class QueryThreadsRequest
    {
        [JsonProperty("user")]
        public UserRequest User { get; set; }

        [JsonProperty("user_id")]
        public string UserID { get; set; }

        [JsonProperty("filter")]
        public Dictionary<string, object> Filter { get; set; }

        [JsonProperty("sort")]
        public List<SortParameter> Sort { get; set; }

        [JsonProperty("watch")]
        public bool? Watch { get; set; }

        [JsonProperty("limit")]
        public int? Limit { get; set; }

        [JsonProperty("offset")]
        public int? Offset { get; set; }
    }

    public class QueryThreadsResponse : ApiResponse
    {
        [JsonProperty("threads")]
        public List<ThreadResponse> Threads { get; set; }
    }
}
