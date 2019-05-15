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
            this._data.AddToJObject(root);
            return root;
        }

        internal static Reaction FromJObject(JObject jObj)
        {
            var result = new Reaction();
            result._data = JsonHelpers.FromJObject(result, jObj);
            var userObj = result._data.GetData<JObject>("user");
            if (userObj != null)
            {
                result.User = User.FromJObject(userObj);
                result._data.RemoveData("user");
            }
            return result;
        }
    }

    public class ReactionResponse
    {

        [JsonIgnore]
        public Message Message { get; internal set; }

        [JsonIgnore]
        public Reaction Reaction { get; internal set; }


        public ReactionResponse() { }

        internal JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.Message != null)
                root.Add("message", this.Message.ToJObject());
            if (this.Reaction != null)
                root.Add("reaction", this.Reaction.ToJObject());
            return root;
        }

        internal static ReactionResponse FromJObject(JObject jObj)
        {
            var result = new ReactionResponse();
            var data = JsonHelpers.FromJObject(result, jObj);
            var msgObj = data.GetData<JObject>("message");
            if (msgObj != null)
                result.Message = Message.FromJObject(msgObj);
            var reactionObj = data.GetData<JObject>("reaction");
            if (reactionObj != null)
                result.Reaction = Reaction.FromJObject(reactionObj);
            return result;
        }
    }
}
