using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public class Reaction : CustomDataBase
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "message_id")]
        public string MessageID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; set; }

        public Reaction() { }

        internal new JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.User != null)
                root.Add("user", this.User.ToJObject());
            return root;
        }

        internal static Reaction FromJObject(JObject jObj)
        {
            var result = new Reaction();
            var data = JsonHelpers.FromJObject(result, jObj);
            var userObj = data.GetData<JObject>("user");
            if (userObj != null)
                result.User = User.FromJObject(userObj);

            return result;
        }
    }
}
