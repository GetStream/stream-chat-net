using System.Collections.Generic;

namespace StreamChat.Models
{
    public class Attachment : CustomDataBase
    {
        public string Type { get; set; }
        public string Fallback { get; set; }
        public string Color { get; set; }
        public string Pretext { get; set; }
        public string AuthorName { get; set; }
        public string AuthorLink { get; set; }
        public string AuthorIcon { get; set; }
        public string Title { get; set; }
        public string TitleLink { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbUrl { get; set; }
        public string Footer { get; set; }
        public string FooterIcon { get; set; }
        public List<AttachmentAction> Actions { get; set; }
        public List<Field> Fields { get; set; }
        public string AssetUrl { get; set; }
        public string OgScrapeUrl { get; set; }
    }

    public class AttachmentAction
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Style { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class Field
    {
        public string Value { get; set; }
        public string Title { get; set; }
        public bool Short { get; set; }
    }
}
