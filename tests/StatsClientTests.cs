using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StreamChat.Models;

namespace StreamChatTests
{
    /// <summary>Tests for <see cref="StatsClient"/></summary>
    /// <remarks>
    /// The tests follow arrange-act-assert pattern divided by empty lines.
    /// Please make sure to follow the pattern to keep the consistency.
    /// </remarks>
    [TestFixture]
    public class StatsClientTests : TestBase
    {
        [Test]
        public async Task TestQueryTeamUsageStatsDefault()
        {
            var response = await _statsClient.QueryTeamUsageStatsAsync();

            response.Teams.Should().NotBeNull();
        }

        [Test]
        public async Task TestQueryTeamUsageStatsWithMonth()
        {
            var currentMonth = DateTime.UtcNow.ToString("yyyy-MM");

            var response = await _statsClient.QueryTeamUsageStatsAsync(new QueryTeamUsageStatsOptions
            {
                Month = currentMonth,
            });

            response.Teams.Should().NotBeNull();
        }

        [Test]
        public async Task TestQueryTeamUsageStatsWithDateRange()
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-7);

            var response = await _statsClient.QueryTeamUsageStatsAsync(new QueryTeamUsageStatsOptions
            {
                StartDate = startDate.ToString("yyyy-MM-dd"),
                EndDate = endDate.ToString("yyyy-MM-dd"),
            });

            response.Teams.Should().NotBeNull();
        }

        [Test]
        public async Task TestQueryTeamUsageStatsWithPagination()
        {
            var response = await _statsClient.QueryTeamUsageStatsAsync(new QueryTeamUsageStatsOptions
            {
                Limit = 10,
            });

            response.Teams.Should().NotBeNull();

            // If there's a next cursor, fetch the next page
            if (!string.IsNullOrEmpty(response.Next))
            {
                var nextResponse = await _statsClient.QueryTeamUsageStatsAsync(new QueryTeamUsageStatsOptions
                {
                    Limit = 10,
                    Next = response.Next,
                });

                nextResponse.Teams.Should().NotBeNull();
            }
        }

        [Test]
        public async Task TestQueryTeamUsageStatsResponseStructure()
        {
            // Query last year to maximize chance of getting data
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-365);

            var response = await _statsClient.QueryTeamUsageStatsAsync(new QueryTeamUsageStatsOptions
            {
                StartDate = startDate.ToString("yyyy-MM-dd"),
                EndDate = endDate.ToString("yyyy-MM-dd"),
            });

            response.Teams.Should().NotBeNull();

            if (response.Teams.Count > 0)
            {
                var team = response.Teams[0];

                // Verify team identifier
                team.Team.Should().NotBeNull();

                // Verify daily activity metrics
                team.UsersDaily.Should().NotBeNull();
                team.MessagesDaily.Should().NotBeNull();
                team.TranslationsDaily.Should().NotBeNull();
                team.ImageModerationsDaily.Should().NotBeNull();

                // Verify peak metrics
                team.ConcurrentUsers.Should().NotBeNull();
                team.ConcurrentConnections.Should().NotBeNull();

                // Verify rolling/cumulative metrics
                team.UsersTotal.Should().NotBeNull();
                team.UsersLast24Hours.Should().NotBeNull();
                team.UsersLast30Days.Should().NotBeNull();
                team.UsersMonthToDate.Should().NotBeNull();
                team.UsersEngagedLast30Days.Should().NotBeNull();
                team.UsersEngagedMonthToDate.Should().NotBeNull();
                team.MessagesTotal.Should().NotBeNull();
                team.MessagesLast24Hours.Should().NotBeNull();
                team.MessagesLast30Days.Should().NotBeNull();
                team.MessagesMonthToDate.Should().NotBeNull();

                // Verify metric structure
                team.UsersDaily.Total.Should().BeGreaterOrEqualTo(0);
            }
        }
    }
}
