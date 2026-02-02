using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="ParsedPredefinedFilterResponse"/>.
    /// </summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class ParsedPredefinedFilterResponseTests
    {
        [Test]
        public void TestDeserializePredefinedFilterResponse()
        {
            var json = @"{
                ""channels"": [],
                ""predefined_filter"": {
                    ""name"": ""user_messaging"",
                    ""filter"": {""type"": ""messaging"", ""members"": {""$in"": [""user123""]}},
                    ""sort"": [{""Field"": ""last_message_at"", ""Direction"": -1}]
                },
                ""duration"": ""0.01s""
            }";

            var response = JsonConvert.DeserializeObject<QueryChannelResponse>(json);

            response.PredefinedFilter.Should().NotBeNull();
            response.PredefinedFilter.Name.Should().Be("user_messaging");
            response.PredefinedFilter.Filter.Should().NotBeNull();
            response.PredefinedFilter.Filter["type"].Should().Be("messaging");
            response.PredefinedFilter.Sort.Should().HaveCount(1);
            response.PredefinedFilter.Sort[0].Field.Should().Be("last_message_at");
        }

        [Test]
        public void TestDeserializeResponseWithoutPredefinedFilter()
        {
            var json = @"{
                ""channels"": [],
                ""duration"": ""0.01s""
            }";

            var response = JsonConvert.DeserializeObject<QueryChannelResponse>(json);

            response.PredefinedFilter.Should().BeNull();
        }

        [Test]
        public void TestDeserializePredefinedFilterWithoutSort()
        {
            var json = @"{
                ""channels"": [],
                ""predefined_filter"": {
                    ""name"": ""simple_filter"",
                    ""filter"": {""type"": ""messaging""}
                },
                ""duration"": ""0.01s""
            }";

            var response = JsonConvert.DeserializeObject<QueryChannelResponse>(json);

            response.PredefinedFilter.Should().NotBeNull();
            response.PredefinedFilter.Name.Should().Be("simple_filter");
            response.PredefinedFilter.Sort.Should().BeNull();
        }
    }
}
