using Microsoft.AspNetCore.Mvc;
using StreamChat.Clients;
using StreamChat.Models;

namespace BackendWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamChannelController : ControllerBase
{
    private readonly IStreamClientFactory _streamClientFactory;

    public StreamChannelController(IStreamClientFactory streamClientFactory)
    {
        _streamClientFactory = streamClientFactory;
    }

    /// <summary>Retrieves a channel. It'll create it if it doesn't exist yet.</summary>
    [HttpGet]
    public async Task<ChannelGetResponse> GetChannelAsync(string channelType, string channelId)
    {
        var channelClient = _streamClientFactory.GetChannelClient();

        return await channelClient.GetOrCreateAsync(channelType, channelId, ChannelGetRequest.WithoutWatching());
    }
}
