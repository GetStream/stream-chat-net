using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public struct MessageType
    {
        public const string Regular = "regular";
        public const string Ephemeral = "ephemeral";
        public const string Error = "error";
        public const string Reply = "reply";
        public const string System = "system";
    }

    public class Message : CustomDataBase
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "id")]
        public string ID { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "text")]
        public string Text { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "command")]
        public string Command { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "html")]
        public string HTML { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "mml")]
        public string MML { get; internal set; }

        [JsonIgnore]
        public User User { get; internal set; }

        [JsonIgnore]
        public List<Attachment> Attachments { get; internal set; }

        [JsonIgnore]
        public List<Reaction> LatestReactions { get; internal set; }

        [JsonIgnore]
        public List<Reaction> OwnReactions { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reaction_counts")]
        public Dictionary<string, int> ReactionCounts { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "parent_id")]
        public string ParentID { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "shown_in_channel")]
        public bool ShownInChannel { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "reply_count")]
        public int? ReplyCount { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "created_at")]
        public DateTime? CreatedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "updated_at")]
        public DateTime? UpdatedAt { get; internal set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "deleted_at")]
        public DateTime? DeletedAt { get; internal set; }

        [JsonIgnore]
        public List<User> MentionedUsers { get; internal set; }


        public Message() { }

        internal new JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            if (this.User != null)
                root.Add("user", this.User.ToJObject());
            if (this.Attachments != null)
                root.Add(new JProperty("attachments", this.Attachments.Select(a => a.ToJObject())));
            if (this.LatestReactions != null)
                root.Add(new JProperty("latest_reactions", this.LatestReactions.Select(r => r.ToJObject())));
            if (this.OwnReactions != null)
                root.Add(new JProperty("own_reactions", this.OwnReactions.Select(r => r.ToJObject())));
            if (this.MentionedUsers != null)
                root.Add(new JProperty("mentioned_users", this.MentionedUsers.Select(u => u.ToJObject())));

            this._data.AddToJObject(root);
            return root;
        }

        internal static Message FromJObject(JObject jObj)
        {
            var result = new Message();
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

            var latestReacts = result._data.GetData<JArray>("latest_reactions");
            if (latestReacts != null)
            {
                var latestReactions = new List<Reaction>();
                foreach (var reaction in latestReacts)
                {
                    var reactionObj = reaction as JObject;
                    latestReactions.Add(Reaction.FromJObject(reactionObj));
                }
                result.LatestReactions = latestReactions;
                result._data.RemoveData("latest_reactions");
            }

            var ownReacts = result._data.GetData<JArray>("own_reactions");
            if (ownReacts != null)
            {
                var ownReactions = new List<Reaction>();
                foreach (var reaction in ownReacts)
                {
                    var reactionObj = reaction as JObject;
                    ownReactions.Add(Reaction.FromJObject(reactionObj));
                }
                result.OwnReactions = ownReactions;
                result._data.RemoveData("own_reactions");
            }

            var mentionedUsrs = result._data.GetData<JArray>("mentioned_users");
            if (mentionedUsrs != null)
            {
                var mentionedUsers = new List<User>();
                foreach (var usr in mentionedUsrs)
                {
                    var usrObj = usr as JObject;
                    mentionedUsers.Add(User.FromJObject(usrObj));
                }
                result.MentionedUsers = mentionedUsers;
                result._data.RemoveData("mentioned_users");
            }

            return result;
        }
    }
}
