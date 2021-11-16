using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;


namespace StreamChat
{

    public class DeleteUsersRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_ids")]
        public string[] UserIDs { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public string User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "messages")]
        public string Messages { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "conversations")]
        public string Conversations { get; set; }

        public DeleteUsersRequest() { }

        public DeleteUsersRequest WithUserIDs(string[] ids)
        {
            this.UserIDs = ids;
            return this;
        }
        public DeleteUsersRequest WithUserDelete(string strategy)
        {
            this.User = strategy;
            return this;
        }

        public DeleteUsersRequest WithMessagesDelete(string strategy)
        {
            this.Messages = strategy;
            return this;
        }

        public DeleteUsersRequest WithConversationsDelete(string strategy)
        {
            this.Conversations = strategy;
            return this;
        }
    }

    public class TaskStatus
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "task_id")]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "result")]
        public Dictionary<string, Object> Result { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public TaskStatus() { }
    }
}
