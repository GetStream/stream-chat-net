using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to retrieve, create and alter custom events of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/custom_events/?language=csharp</remarks>
    public interface IEventClient
    {
        /// <summary>
        /// <para>Sends a custom event to a channel.</para>
        /// Custom events allow you to build more complex interactions within a channel or with a user.
        /// Users connected to a channel, either as a watcher or member, can send
        /// custom events and have them delivered to all users watching the channel.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/custom_events/?language=csharp</remarks>
        Task<SendEventResponse> SendEventAsync(string channelType, string channelId, Event @event);

        /// <summary>
        /// <para>Sends a custom event to a user.</para>
        /// This allows you to send custom events to a connected user. The event is delivered
        /// to all connected clients for that user. It is only available with server-side authentication.
        /// A copy of the event is sent via web-hooks if it is enabled.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/custom_events/?language=csharp</remarks>
        Task<ApiResponse> SendUserCustomEventAsync(string userId, UserCustomEvent @event);
    }
}