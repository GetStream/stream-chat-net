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
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        Task<ApiResponse> FlagMessageAsync(string flaggedId, string flaggerID);

        /// <summary>
        /// <para>Flags a user.</para>
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        Task<ApiResponse> FlagUserAsync(string flaggedId, string flaggerId);

        /// <summary>
        /// <para>Queries message flags.</para>
        /// If you prefer to build your own in app moderation dashboard, rather than use the Stream
        /// dashboard, then the query message flags endpoint lets you get flagged messages. Similar
        /// to other queries in Stream Chat, you can filter the flags using query operators.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp#query-message-flags</remarks>
        Task<QueryMessageFlagsResponse> QueryMessageFlags(FlagMessageQueryRequest request);

        /// <summary>
        /// Queries flag reports.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        Task<QueryFlagReportsResponse> QueryFlagReportsAsync(QueryFlagReportsRequest request);

        /// <summary>
        /// Sends a flag report review.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/moderation/?language=csharp</remarks>
        Task<ReviewFlagReportResponse> ReviewFlagReportAsync(string reportId, ReviewFlagReportRequest request);
    }
}