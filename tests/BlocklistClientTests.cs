using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="BlocklistClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// Note: Create and Delete methods are not tested explicitly, because
    /// we use them in the setup and teardown already.
    /// </remarks>
    [TestFixture]
    public class BlocklistClientTests : TestBase
    {
        private string _blocklistName;

        [OneTimeSetUp]
        public async Task SetupAsync()
        {
            _blocklistName = Guid.NewGuid().ToString();
            await _blocklistClient.CreateAsync(new BlocklistCreateRequest
            {
                Name = _blocklistName,
                Words = new[] { "test" },
            });
        }

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
            await _blocklistClient.DeleteAsync(_blocklistName);
        }

        [Test]
        public Task TestGetAsync() => TryMultiple(async () =>
        {
            var resp = await _blocklistClient.GetAsync(_blocklistName);

            resp.Blocklist.Name.Should().Be(_blocklistName);
            resp.Blocklist.Words.Should().NotBeEmpty();
            resp.Blocklist.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));
            resp.Blocklist.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(10));
        });

        [Test]
        public Task TestListAsync() => TryMultiple(async () =>
        {
            var resp = await _blocklistClient.ListAsync();

            resp.Blocklists.Should().Contain(x => x.Name == _blocklistName);
        });

        [Test]
        public async Task TestUpdateAsync()
        {
            var expectedWords = new[] { "test", "test2" };

            await TryMultiple(async () =>
            {
                await _blocklistClient.UpdateAsync(_blocklistName, expectedWords);
            });

            await TryMultiple(async () =>
            {
                var updated = await _blocklistClient.GetAsync(_blocklistName);
                updated.Blocklist.Words.Should().BeEquivalentTo(expectedWords);
            });
        }
    }
}