using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    public interface IThreadClient
    {
        Task<QueryThreadsResponse> QueryThreads(QueryThreadsOptions opts);
    }
}
