using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public class ExportedUser
    {

        [JsonIgnore]
        public User User { get; internal set; }

        [JsonIgnore]
        public List<Message> Messages { get; internal set; }

        [JsonIgnore]
        public List<Reaction> Reactions { get; internal set; }

        public ExportedUser() { }


        internal JObject ToJObject()
        {
            var root = new JObject();
            if (this.User != null)
                root.Add("user", this.User.ToJObject());
            if (this.Messages != null)
                root.Add(new JProperty("messages", this.Messages.Select(a => a.ToJObject())));
            if (this.Reactions != null)
                root.Add(new JProperty("reactions", this.Reactions.Select(r => r.ToJObject())));

            return root;
        }

        internal static ExportedUser FromJObject(JObject jObj)
        {
            var result = new ExportedUser();
            var data = JsonHelpers.FromJObject(result, jObj);

            var userObj = data.GetData<JObject>("user");
            if (userObj != null)
                result.User = User.FromJObject(userObj);

            var reacts = data.GetData<JArray>("reactions");
            if (reacts != null)
            {
                var reactions = new List<Reaction>();
                foreach (var reaction in reacts)
                {
                    var reactionObj = reaction as JObject;
                    reactions.Add(Reaction.FromJObject(reactionObj));
                }
                result.Reactions = reactions;
            }

            var msgs = data.GetData<JArray>("messages");
            if (msgs != null)
            {
                var messages = new List<Message>();
                foreach (var msg in msgs)
                {
                    var msgObj = msg as JObject;
                    messages.Add(Message.FromJObject(msgObj));
                }
                result.Messages = messages;
            }

            return result;
        }
    }
}
