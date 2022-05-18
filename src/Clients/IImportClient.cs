using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to create and list imports.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/import/?language=csharp</remarks>
    public interface IImportClient
    {
        /// <summary>
        /// Creates an import URL. It returns an S3 signed URL (<see cref="CreateImportUrlResponse.UploadUrl"/>) which you can use to
        /// upload the file with a "PUT" request.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/import/?language=csharp</remarks>
        Task<CreateImportUrlResponse> CreateImportUrlAsync(string fileName);

        /// <summary>
        /// Creates an import.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/import/?language=csharp</remarks>
        Task<CreateImportResponse> CreateImportAsync(string path, ImportMode mode);

        /// <summary>
        /// Returns an import.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/import/?language=csharp</remarks>
        Task<GetImportResponse> GetImportAsync(string id);

        /// <summary>
        /// List all imports. You can paginate the results using <paramref name="options"/> parameter.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/import/?language=csharp</remarks>
        Task<ListImportsResponse> ListImportsAsync(ListImportsOptions options = null);
    }
}