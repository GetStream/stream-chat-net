using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public class Read
    {
        [JsonIgnore]
        public User User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "last_read")]
        public DateTime? LastRead { get; set; }

        public Read() { }

        internal JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.User != null)
                root.Add("user", this.User.ToJObject());
            return root;
        }

        internal static Read FromJObject(JObject jObj)
        {
            var result = new Read();
            var data = JsonHelpers.FromJObject(result, jObj);
            var userObj = data.GetData<JObject>("user");
            if (userObj != null)
                result.User = User.FromJObject(userObj);
            return result;
        }
    }
}
