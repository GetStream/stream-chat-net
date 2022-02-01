using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to retrieve and alter custom commands of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/custom_commands_webhook/?language=csharp</remarks>
    public interface ICommandClient
    {
        /// <summary>
        /// <para>Creates a custom command</para>
        /// By using custom commands, you can receive all messages sent using a specific slash command,
        /// eg. /ticket, in your application. When configured, every slash command message happening
        /// in a Stream Chat application will propagate to an endpoint via an HTTP POST request.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/custom_commands_webhook/?language=csharp</remarks>
        Task<CommandResponse> CreateAsync(CommandCreateRequest request);

        /// <summary>
        /// <para>Deletes a custom command</para>
        /// By using custom commands, you can receive all messages sent using a specific slash command,
        /// eg. /ticket, in your application. When configured, every slash command message happening
        /// in a Stream Chat application will propagate to an endpoint via an HTTP POST request.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/custom_commands_webhook/?language=csharp</remarks>
        Task<ApiResponse> DeleteAsync(string name);

        /// <summary>
        /// <para>Returns a custom command</para>
        /// By using custom commands, you can receive all messages sent using a specific slash command,
        /// eg. /ticket, in your application. When configured, every slash command message happening
        /// in a Stream Chat application will propagate to an endpoint via an HTTP POST request.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/custom_commands_webhook/?language=csharp</remarks>
        Task<CommandGetResponse> GetAsync(string name);

        /// <summary>
        /// <para>Lists all custom commands</para>
        /// By using custom commands, you can receive all messages sent using a specific slash command,
        /// eg. /ticket, in your application. When configured, every slash command message happening
        /// in a Stream Chat application will propagate to an endpoint via an HTTP POST request.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/custom_commands_webhook/?language=csharp</remarks>
        Task<ListCommandsResponse> ListAsync();

        /// <summary>
        /// <para>Updates a custom command</para>
        /// By using custom commands, you can receive all messages sent using a specific slash command,
        /// eg. /ticket, in your application. When configured, every slash command message happening
        /// in a Stream Chat application will propagate to an endpoint via an HTTP POST request.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/custom_commands_webhook/?language=csharp</remarks>
        Task<CommandResponse> UpdateAsync(string name, CommandUpdateRequest request);
    }
}