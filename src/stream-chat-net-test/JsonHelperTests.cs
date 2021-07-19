using Newtonsoft.Json.Linq;
using NUnit.Framework;
using StreamChat;
using System;
using System.Linq;

namespace StreamChatTests
{
    [TestFixture]
    public class JsonHelperTests
    {
        [TestCase]
        public void TestCorrectness()
        {
            var orig = CreateTestAttachment();

            var testObject = orig.ToJObject();

            var now = DateTime.Now;
            var testPropName = "TestProperty";

            testObject.Add(testPropName, JToken.FromObject(now));

            var copy = new Attachment();

            var extraData = JsonHelpers.FromJObject(copy, testObject);

            Assert.IsTrue(AreEqual(orig, copy));
            Assert.AreEqual(now, extraData.GetData<DateTime>(testPropName));
        }

        private static Attachment CreateTestAttachment()
        {
            return new Attachment
            {
                Actions = new System.Collections.Generic.List<AttachmentAction>
                {
                    new AttachmentAction
                    {
                        Name = "AttachmentAction",
                        Style = "AttachmentActionStyle",
                        Text = "AttachmentActionText",
                        Type = "AttachmentActionType",
                        Value = "AttachmentActionValue"
                    }
                },
                AssetURL = "AssetURL",
                Type = "Type",
                Text = "Text",
                AuthorIcon = "AuthorIcon",
                AuthorLink = "AuthorLink",
                AuthorName = "AuthorName",
                Color = "Color",
                Fallback = "Fallback",
                Fields = new System.Collections.Generic.List<Field>
                {
                    new Field
                    {
                        Short = true,
                        Title = "Title",
                        Value = "Value"
                    }
                },
                Title = "Title",
                Footer = "Footer",
                FooterIcon = "FooterIcon ",
                ImageURL = "ImageURL",
                OGScrapeUrl = "OGScrapeUrl ",
                Pretext = "Pretext",
                ThumbURL = "ThumbURL",
                TitleLink = null
            };
        }

        private bool AreEqual(Attachment orig, Attachment copy)
        {
            return orig.Actions
                .Zip(copy.Actions, (a1, a2) =>
                    a1.Name == a2.Name
                    && a1.Style == a2.Style
                    && a1.Text == a2.Text
                    && a1.Type == a2.Type
                    && a1.Value == a2.Value
                    )
                .All(x => x)
                && orig.AssetURL == copy.AssetURL
                && orig.AuthorIcon == copy.AuthorIcon
                && orig.AuthorLink == copy.AuthorLink
                && orig.AuthorName == copy.AuthorName
                && orig.Color == copy.Color
                && orig.Fallback == copy.Fallback
                && orig.Fields
                .Zip(copy.Fields, (f1, f2) =>
                    f1.Short == f2.Short
                    && f1.Title == f2.Title
                    && f1.Value == f2.Value
                    )
                .All(x => x)
                && orig.Footer == copy.Footer
                && orig.FooterIcon == copy.FooterIcon
                && orig.ImageURL == copy.ImageURL
                && orig.OGScrapeUrl == copy.OGScrapeUrl
                && orig.Pretext == copy.Pretext
                && orig.Text == copy.Text
                && orig.ThumbURL == copy.ThumbURL
                && orig.Title == copy.Title
                && orig.TitleLink == copy.TitleLink
                && orig.Type == copy.Type;
        }
    }
}
