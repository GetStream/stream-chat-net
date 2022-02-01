using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to retrieve, create and alter reactions of messages.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_reaction/?language=csharp</remarks>
    public interface IReactionClient
    {
        /// <summary>
        /// Deletes a reaction from a given message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_reaction/?language=csharp</remarks>
        Task<ReactionResponse> DeleteReactionAsync(string messageId, string reactionType, string userId);

        /// <summary>
        /// <para>Returns a reaction of a given message.</para>
        /// Messages returned by the APIs automatically include the 10 most recent reactions.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_reaction/?language=csharp</remarks>
        Task<GetReactionsResponse> GetReactionsAsync(string messageId, int offset = 0, int limit = 50);

        /// <summary>
        /// <para>Sends a new reaction to a given message.</para>
        /// <paramref name="userId"/> is the id of a user who reacted to the message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_reaction/?language=csharp</remarks>
        Task<ReactionResponse> SendReactionAsync(string messageId, string reactionType, string userId);

        /// <summary>
        /// <para>Sends a new reaction to a given message.</para>
        /// <paramref name="userId"/> is the id of a user who reacted to the message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_reaction/?language=csharp</remarks>
        Task<ReactionResponse> SendReactionAsync(string messageId, string reactionType, string userId, bool skipPush);

        /// <summary>
        /// <para>Sends a new reaction to a given message.</para>
        /// Make sure that <see cref="ReactionRequest.UserId"/> field of
        /// <paramref name="reaction"/> parameter is not empty. That is the id of the user who
        /// reacted to the message.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/send_reaction/?language=csharp</remarks>
        Task<ReactionResponse> SendReactionAsync(string messageId, ReactionRequest reaction, bool skipPush = false);
    }
}