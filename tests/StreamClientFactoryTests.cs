using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Clients;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="StreamClientFactory"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class StreamClientFactoryTests : TestBase
    {
        [Test]
        public void TestStreamKeyEmptyThrowsError()
        {
            Action keyEmptyOperation = () => new StreamClientFactory(apiKey: string.Empty, apiSecret: "secret");
            Action secretEmptyOperation = () => new StreamClientFactory(apiKey: "key", apiSecret: string.Empty);

            keyEmptyOperation.Should().Throw<ArgumentNullException>();
            secretEmptyOperation.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public async Task TestStreamHttpClientIsBeingUsedAsync()
        {
            var testHandler = new TestHttpMessageHandler();
            var httpClient = new HttpClient(testHandler);
            var factory = new StreamClientFactory("key", "secret", opts => opts.HttpClient = httpClient);
            var appClient = factory.GetAppClient();

            await appClient.GetAppSettingsAsync();

            testHandler.InvocationCounter.Should().Be(1);
        }
    }

    public class TestHttpMessageHandler : HttpMessageHandler
    {
        public int InvocationCounter { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            InvocationCounter++;

            return Task.FromResult(new HttpResponseMessage());
        }
    }
}