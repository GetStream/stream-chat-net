using System.Net;
using System.Threading.Tasks;
using StreamChat.Models;
using StreamChat.Rest;
using HttpMethod = StreamChat.Rest.HttpMethod;

namespace StreamChat.Clients
{
    public class ImportClient : ClientBase, IImportClient
    {
        internal ImportClient(IRestClient client) : base(client)
        {
        }

        public async Task<CreateImportUrlResponse> CreateImportUrlAsync(string fileName)
          => await ExecuteRequestAsync<CreateImportUrlResponse>("import_urls",
              HttpMethod.POST,
              HttpStatusCode.Created,
              new { filename = fileName });

        public async Task<CreateImportResponse> CreateImportAsync(string path, ImportMode mode)
            => await ExecuteRequestAsync<CreateImportResponse>("imports",
                HttpMethod.POST,
                HttpStatusCode.Created,
                new { path, mode });

        public async Task<GetImportResponse> GetImportAsync(string id)
            => await ExecuteRequestAsync<GetImportResponse>($"imports/{id}",
                HttpMethod.GET,
                HttpStatusCode.OK);

        public async Task<ListImportsResponse> ListImportsAsync(ListImportsOptions options = null)
            => await ExecuteRequestAsync<ListImportsResponse>("imports",
                HttpMethod.GET,
                HttpStatusCode.OK,
                queryParams: options?.ToQueryParameters());
    }
}