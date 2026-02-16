using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to query team-level usage statistics.
    /// </summary>
    public interface IStatsClient
    {
        /// <summary>
        /// <para>Queries team-level usage statistics from the warehouse database.</para>
        /// Returns all 16 metrics grouped by team with cursor-based pagination.
        /// This endpoint is server-side only.
        /// <para>Date Range Options (mutually exclusive):</para>
        /// <list type="bullet">
        /// <item>Use 'month' parameter (YYYY-MM format) for monthly aggregated values</item>
        /// <item>Use 'start_date'/'end_date' parameters (YYYY-MM-DD format) for daily breakdown</item>
        /// <item>If neither provided, defaults to current month (monthly mode)</item>
        /// </list>
        /// </summary>
        Task<QueryTeamUsageStatsResponse> QueryTeamUsageStatsAsync(QueryTeamUsageStatsOptions options = null);
    }
}
