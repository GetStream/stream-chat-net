using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="ImportClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class ImportClientTests : TestBase
    {
        private CreateImportUrlResponse _urlResp;

        [OneTimeSetUp]
        public async Task OneTimeSetupAsync()
        {
            _urlResp = await _importClient.CreateImportUrlAsync("streamchatnet.json");
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(_urlResp.UploadUrl),
                    Method = HttpMethod.Put,
                    Content = new StringContent("{}"),
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (request)
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        [Test]
        public async Task TestImportsEnd2EndAsync()
        {
            var createResp = await _importClient.CreateImportAsync(_urlResp.Path, ImportMode.Upsert);
            createResp.ImportTask.Id.Should().NotBeNullOrEmpty();
            createResp.ImportTask.Path.Should().Contain(_urlResp.Path);

            var importResp = await _importClient.GetImportAsync(createResp.ImportTask.Id);
            importResp.ImportTask.Id.Should().BeEquivalentTo(createResp.ImportTask.Id);

            var listResp = await _importClient.ListImportsAsync(new ListImportsOptions { Limit = 1 });
            listResp.ImportTasks.Should().HaveCount(1);
        }
    }
}