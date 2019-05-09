using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public class ChannelObject : CustomDataBase
    {
        public ChannelObject() { }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "cid")]
        public string CID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "last_message_at")]
        public DateTime? LastMessageAt { get; set; }

        [JsonIgnore]
        public User CreatedBy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "frozen")]
        public bool Frozen { get; set; }

        [JsonIgnore]
        public List<ChannelMember> Members { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "member_count")]
        public int MemberCount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Invites")]
        public List<string> Invites { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "config")]
        public ChannelConfig Config { get; set; }

        internal new JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.Members != null)
                root.Add(new JProperty("members", this.Members.Select(x => x.ToJObject())));
            if (this.CreatedBy != null)
                root.Add("created_by", this.CreatedBy.ToJObject());
            return root;
        }

        internal static ChannelObject FromJObject(JObject jObj)
        {
            var result = new ChannelObject();
            result._data = JsonHelpers.FromJObject(result, jObj);

            var mbrs = result._data.GetData<JArray>("members");
            if (mbrs != null)
            {
                var members = new List<ChannelMember>();
                foreach (var mbr in mbrs)
                {
                    var memberObj = mbr as JObject;
                    members.Add(ChannelMember.FromJObject(memberObj));
                }
                result.Members = members;
                result._data.RemoveData("members");
            }

            var userObj = result._data.GetData<JObject>("created_by");
            if (userObj != null)
            {
                result.CreatedBy = User.FromJObject(userObj);
                result._data.RemoveData("created_by");
            }

            return result;
        }
    }
}
