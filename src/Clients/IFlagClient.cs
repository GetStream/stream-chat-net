using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to moderate messages or users of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
    public interface IFlagClient
    {
        /// <summary>
        /// <para>Flags a message.</para>
        /// Any user is allowed to flag a message. This triggers the
        /// message.flagged webhook event and adds the message to the inbox of your Stream Dashboard Chat Moderation view.
        /// To unflag a message, use <see cref="UnflagMessageAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        Task<ApiResponse> FlagMessageAsync(string flaggedId, string flaggerID);

        /// <summary>
        /// <para>Unflags a message.</para>
        /// Any user is allowed to flag a message. This triggers the
        /// message.flagged webhook event and adds the message to the inbox of your Stream Dashboard Chat Moderation view.
        /// To flag a message, use <see cref="FlagMessageAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        Task<ApiResponse> UnflagMessageAsync(string flaggedId, string flaggerId);

        /// <summary>
        /// <para>Flags a user.</para>
        /// To unflag a user, use <see cref="UnflagUserAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        Task<ApiResponse> FlagUserAsync(string flaggedId, string flaggerId);

        /// <summary>
        /// <para>Unflags a user.</para>
        /// To flag a user, use <see cref="FlagUserAsync"/> method.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        Task<ApiResponse> UnflagUserAsync(string flaggedId, string flaggerId);

        /// <summary>
        /// <para>Queries message flags.</para>
        /// If you prefer to build your own in app moderation dashboard, rather than use the Stream
        /// dashboard, then the query message flags endpoint lets you get flagged messages. Similar
        /// to other queries in Stream Chat, you can filter the flags using query operators.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp#query-message-flags</remarks>
        Task<QueryMessageFlagsResponse> QueryMessageFlags(FlagMessageQueryRequest request);
    }
}