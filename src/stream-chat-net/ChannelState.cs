using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public class ChannelState
    {
        public ChannelState() { }

        [JsonIgnore]
        public ChannelObjectWithInfo Channel { get; internal set; }

        [JsonIgnore]
        public List<Message> Messages { get; internal set; }

        [JsonIgnore]
        public List<User> Watchers { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "watcher_count")]
        public int WatcherCount { get; internal set; }

        [JsonIgnore]
        public List<Read> Reads { get; internal set; }

        [JsonIgnore]
        public List<ChannelMember> Members { get; internal set; }


        internal JObject ToJObject()
        {
            var root = JObject.FromObject(this);

            if (this.Channel != null)
                root.Add("channel", this.Channel.ToJObject());

            if (this.Messages != null)
                root.Add(new JProperty("messages", this.Messages.Select(x => x.ToJObject())));

            if (this.Watchers != null)
                root.Add(new JProperty("watchers", this.Watchers.Select(x => x.ToJObject())));

            if (this.Reads != null)
                root.Add(new JProperty("read", this.Reads.Select(x => x.ToJObject())));

            if (this.Members != null)
                root.Add(new JProperty("members", this.Members.Select(x => x.ToJObject())));

            return root;
        }

        internal static ChannelState FromJObject(JObject jObj)
        {
            var result = new ChannelState();
            var data = JsonHelpers.FromJObject(result, jObj);

            var chanObj = data.GetData<JObject>("channel");
            if (chanObj != null)
                result.Channel = ChannelObjectWithInfo.FromJObject(chanObj);

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

            var usrs = data.GetData<JArray>("watchers");
            if (usrs != null)
            {
                var users = new List<User>();
                foreach (var usr in usrs)
                {
                    var usrObj = usr as JObject;
                    users.Add(User.FromJObject(usrObj));
                }
                result.Watchers = users;
            }

            var reads = data.GetData<JArray>("read");
            if (reads != null)
            {
                var readsList = new List<Read>();
                foreach (var read in reads)
                {
                    var readObj = read as JObject;
                    readsList.Add(Read.FromJObject(readObj));
                }
                result.Reads = readsList;
            }

            var mbrs = data.GetData<JArray>("members");
            if (mbrs != null)
            {
                var members = new List<ChannelMember>();
                foreach (var mbr in mbrs)
                {
                    var memberObj = mbr as JObject;
                    members.Add(ChannelMember.FromJObject(memberObj));
                }
                result.Members = members;
            }

            return result;
        }
    }
}
