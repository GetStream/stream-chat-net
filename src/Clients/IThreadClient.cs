using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// Client for managing thread-related operations in Stream Chat.
    /// </summary>
    public interface IThreadClient
    {
        /// <summary>
        /// Queries threads based on the provided options including filtering, sorting, and pagination.
        /// </summary>
        /// <param name="opts">Options for querying threads including filters, sort parameters, and pagination settings.</param>
        /// <returns>A response containing the queried threads.</returns>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/threads/?language=csharp#filtering-and-sorting-threads</remarks>
        Task<QueryThreadsResponse> QueryThreadsAsync(QueryThreadsOptions opts);
    }
}
