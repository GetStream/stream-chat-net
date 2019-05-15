using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public class Mute
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; internal set; }

        [JsonIgnore]
        public User User { get; internal set; }

        [JsonIgnore]
        public User Target { get; internal set; }

        public Mute() { }

        internal JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.User != null)
                root.Add("user", this.User.ToJObject());
            if (this.Target != null)
                root.Add("target", this.Target.ToJObject());
            return root;
        }

        internal static Mute FromJObject(JObject jObj)
        {
            var result = new Mute();
            var data = JsonHelpers.FromJObject(result, jObj);
            var userObj = data.GetData<JObject>("user");
            if (userObj != null)
                result.User = User.FromJObject(userObj);
            userObj = data.GetData<JObject>("target");
            if (userObj != null)
                result.Target = User.FromJObject(userObj);

            return result;
        }
    }

    public class MuteResponse
    {
        [JsonIgnore]
        public User OwnUser { get; internal set; }


        [JsonIgnore]
        public Mute Mute { get; internal set; }

        public MuteResponse() { }

        internal JObject ToJObject()
        {
            var root = new JObject();
            if (this.OwnUser != null)
                root.Add("own_user", this.OwnUser.ToJObject());
            if (this.Mute != null)
                root.Add("mute", this.Mute.ToJObject());
            return root;
        }

        internal static MuteResponse FromJObject(JObject jObj)
        {
            var result = new MuteResponse();
            var data = JsonHelpers.FromJObject(result, jObj);
            var userObj = data.GetData<JObject>("own_user");
            if (userObj != null)
                result.OwnUser = User.FromJObject(userObj);

            var muteObj = data.GetData<JObject>("mute");
            if (muteObj != null)
                result.Mute = Mute.FromJObject(muteObj);

            return result;
        }

    }
}
