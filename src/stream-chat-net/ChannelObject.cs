using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public class ChannelObject : CustomDataBase
    {
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

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "invites")]
        public List<string> Invites { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "config")]
        public ChannelTypeWithCommands Config { get; set; }

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

    public class ChannelObjectWithInfo : ChannelObject
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "config")]
        public new ChannelTypeWithCommands Config { get; set; }

        internal new static ChannelObjectWithInfo FromJObject(JObject jObj)
        {
            var result = new ChannelObjectWithInfo();
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

    public class UpdateChannelResponse
    {

        [JsonIgnore]
        public ChannelObjectWithInfo Channel { get; internal set; }

        [JsonIgnore]
        public Message Message { get; internal set; }

        internal JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.Message != null)
                root.Add("message", this.Message.ToJObject());
            if (this.Channel != null)
                root.Add("channel", this.Channel.ToJObject());
            return root;
        }

        internal static UpdateChannelResponse FromJObject(JObject jObj)
        {
            var result = new UpdateChannelResponse();
            var data = JsonHelpers.FromJObject(result, jObj);
            var msgObj = data.GetData<JObject>("message");
            if (msgObj != null)
                result.Message = Message.FromJObject(msgObj);
            var chanObj = data.GetData<JObject>("channel");
            if (chanObj != null)
                result.Channel = ChannelObjectWithInfo.FromJObject(chanObj);
            return result;
        }
    }

    public class PartialUpdateChannelRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "set")]
        public Dictionary<string, object> Set { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "unset")]
        public IEnumerable<string> Unset { get; set; }

        internal JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.User != null)
                root.Add("user", this.User.ToJObject());
            return root;
        }
    }

    public class PartialUpdateChannelResponse
    {
        [JsonIgnore]
        public ChannelObjectWithInfo Channel { get; internal set; }

        [JsonIgnore]
        public List<ChannelMember> Members { get; internal set; }

        internal static PartialUpdateChannelResponse FromJObject(JObject jObj)
        {
            var result = new PartialUpdateChannelResponse();
            var data = JsonHelpers.FromJObject(result, jObj);

            var chanObj = data.GetData<JObject>("channel");
            if (chanObj != null)
                result.Channel = ChannelObjectWithInfo.FromJObject(chanObj);

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

    public class TruncateOptions
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "hard_delete")]
        public bool? HardDelete { get; set; }

        [JsonIgnore]
        public MessageInput Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "skip_push")]
        public bool? SkipPush { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "truncated_at")]
        public DateTime? TruncatedAt { get; set; }

        internal JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.Message != null)
                root.Add("message", this.Message.ToJObject());
            return root;
        }
    }

    public class TruncateResponse
    {
        public string Duration { get; set; }
        public Message Message { get; set; }
        public ChannelObjectWithInfo Channel { get; set; }

        internal static TruncateResponse FromJObject(JObject jObj)
        {
            var result = new TruncateResponse();

            var data = JsonHelpers.FromJObject(result, jObj);
            var msgObj = data.GetData<JObject>("message");
            if (msgObj != null)
                result.Message = Message.FromJObject(msgObj);
            var chanObj = data.GetData<JObject>("channel");
            if (chanObj != null)
                result.Channel = ChannelObjectWithInfo.FromJObject(chanObj);
            result.Duration = data.GetData<string>("duration");

            return result;
        }
    }
}
