using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="ApiResponse"/>.
    /// </summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class ApiResponseTests : TestBase
    {
        [Test]
        public async Task TestResponseContainsProperFieldsAsync()
        {
            var resp = await _appClient.GetAppSettingsAsync();

            resp.Duration.Should().NotBeNullOrWhiteSpace();
            var rateLimit = resp.GetRateLimit();
            rateLimit.Limit.Should().BeGreaterThan(0);
            rateLimit.Remaining.Should().BeGreaterThan(0);
            rateLimit.Reset.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(2));
        }
    }
}