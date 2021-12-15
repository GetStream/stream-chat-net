using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public class MessageInput : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mml")]
        public string Mml { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "html")]
        public string HTML { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "parent_id")]
        public string ParentID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "shown_in_channel")]
        public bool? ShownInChannel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mentioned_users")]
        public List<string> MentionedUsers { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "silent")]
        public bool? Silent { get; set; }

        internal new JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.User != null)
                root.Add("user", this.User.ToJObject());
            if (this.Attachments != null)
                root.Add(new JProperty("attachments", this.Attachments.Select(a => a.ToJObject())));

            this._data.AddToJObject(root);
            return root;
        }

        internal static MessageInput FromJObject(JObject jObj)
        {
            var result = new MessageInput();
            result._data = JsonHelpers.FromJObject(result, jObj);
            var userObj = result._data.GetData<JObject>("user");
            if (userObj != null)
            {
                result.User = User.FromJObject(userObj);
                result._data.RemoveData("user");
            }

            var attachs = result._data.GetData<JArray>("attachments");
            if (attachs != null)
            {
                var attachments = new List<Attachment>();
                foreach (var att in attachs)
                {
                    var attachObj = att as JObject;
                    attachments.Add(Attachment.FromJObject(attachObj));
                }
                result.Attachments = attachments;
                result._data.RemoveData("attachments");
            }
            return result;
        }
    }
}
