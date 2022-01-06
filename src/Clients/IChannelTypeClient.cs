using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to retrieve, create and alter channel types of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_features/?language=csharp</remarks>
    public interface IChannelTypeClient
    {
        /// <summary>
        /// Creates a channel type.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_members/?language=csharp</remarks>
        Task<ChannelTypeWithStringCommandsResponse> CreateChannelTypeAsync(ChannelTypeWithStringCommandsRequest channelType);

        /// <summary>
        /// Deletes a channel type.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_features/?language=csharp/</remarks>
        Task<ApiResponse> DeleteChannelTypeAsync(string type);

        /// <summary>
        /// Returns a channel type.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_features/?language=csharp/</remarks>
        Task<ChannelTypeWithCommandsResponse> GetChannelTypeAsync(string type);

        /// <summary>
        /// Returns all of the available channel types.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_features/?language=csharp/</remarks>
        Task<ListChannelTypesResponse> ListChannelTypesAsync();

        /// <summary>
        /// <para>Updates a channel type.</para>
        /// Channel type features, commands and permissions can be changed. Only the fields that must
        /// change need to be provided, fields that are not provided to this API will remain unchanged.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/channel_features/?language=csharp/</remarks>
        Task<ChannelTypeWithStringCommandsResponse> UpdateChannelTypeAsync(string type, ChannelTypeWithStringCommandsRequest updateReq);
    }
}