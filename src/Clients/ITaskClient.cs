using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to interract with asynchronous operations of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/rest/#tasks-gettask</remarks>
    public interface ITaskClient
    {
        /// <summary>
        /// Returns the status a task.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/rest/#tasks-gettask</remarks>
        Task<AsyncTaskStatusResponse> GetTaskStatusAsync(string taskId);
    }
}