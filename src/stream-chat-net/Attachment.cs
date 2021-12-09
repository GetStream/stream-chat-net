using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public class Attachment : CustomDataBase
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "fallback")]
        public string Fallback { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "pretext")]
        public string Pretext { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "author_name")]
        public string AuthorName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "author_link")]
        public string AuthorLink { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "autor_icon")]
        public string AuthorIcon { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "title_link")]
        public string TitleLink { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "image_url")]
        public string ImageURL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "thumb_url")]
        public string ThumbURL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "footer")]
        public string Footer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "footer_icon")]
        public string FooterIcon { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "actions")]
        public List<AttachmentAction> Actions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "fields")]
        public List<Field> Fields { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "asset_url")]
        public string AssetURL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "og_scrape_url")]
        public string OGScrapeUrl { get; set; }

        internal static Attachment FromJObject(JObject jObj)
        {
            var result = new Attachment();
            result._data = JsonHelpers.FromJObject(result, jObj);
            return result;
        }
    }

    public class AttachmentAction
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "style")]
        public string Style { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "value")]
        public string Value { get; set; }  
    }

    public class Field
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "short")]
        public bool Short { get; set; }  
    }
}
