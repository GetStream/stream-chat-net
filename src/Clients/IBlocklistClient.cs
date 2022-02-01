using System.Collections.Generic;
using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to retrieve and alter blocklists of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/block_lists/?language=csharp</remarks>
    public interface IBlocklistClient
    {
        /// <summary>
        /// <para>Creates a blocklist</para>
        /// A Block List is a list of words that you can use to moderate chat messages. Stream Chat
        /// comes with a built-in Block List called profanity_en_2020_v1 which contains over a thousand
        /// of the most common profane words.
        /// You can manage your own block lists via the Stream dashboard or APIs to a manage
        /// blocklists and configure your channel types to use them.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/block_lists/?language=csharp</remarks>
        Task<ApiResponse> CreateAsync(BlocklistCreateRequest request);

        /// <summary>
        /// <para>Deletes a blocklist</para>
        /// A Block List is a list of words that you can use to moderate chat messages. Stream Chat
        /// comes with a built-in Block List called profanity_en_2020_v1 which contains over a thousand
        /// of the most common profane words.
        /// You can manage your own block lists via the Stream dashboard or APIs to a manage
        /// blocklists and configure your channel types to use them.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/block_lists/?language=csharp</remarks>
        Task<ApiResponse> DeleteAsync(string name);

        /// <summary>
        /// <para>Gets a blocklist</para>
        /// A Block List is a list of words that you can use to moderate chat messages. Stream Chat
        /// comes with a built-in Block List called profanity_en_2020_v1 which contains over a thousand
        /// of the most common profane words.
        /// You can manage your own block lists via the Stream dashboard or APIs to a manage
        /// blocklists and configure your channel types to use them.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/block_lists/?language=csharp</remarks>
        Task<GetBlocklistResponse> GetAsync(string name);

        /// <summary>
        /// <para>Returns all blocklists</para>
        /// A Block List is a list of words that you can use to moderate chat messages. Stream Chat
        /// comes with a built-in Block List called profanity_en_2020_v1 which contains over a thousand
        /// of the most common profane words.
        /// You can manage your own block lists via the Stream dashboard or APIs to a manage
        /// blocklists and configure your channel types to use them.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/block_lists/?language=csharp</remarks>
        Task<ListBlocklistsResponse> ListAsync();

        /// <summary>
        /// <para>Updates a blocklist</para>
        /// A Block List is a list of words that you can use to moderate chat messages. Stream Chat
        /// comes with a built-in Block List called profanity_en_2020_v1 which contains over a thousand
        /// of the most common profane words.
        /// You can manage your own block lists via the Stream dashboard or APIs to a manage
        /// blocklists and configure your channel types to use them.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/block_lists/?language=csharp</remarks>
        Task<ApiResponse> UpdateAsync(string name, IEnumerable<string> words);
    }
}