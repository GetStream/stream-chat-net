using Microsoft.AspNetCore.Mvc;
using StreamChat.Clients;

namespace BackendWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamTokenController : ControllerBase
{
    private readonly IStreamClientFactory _streamClientFactory;

    public StreamTokenController(IStreamClientFactory streamClientFactory)
    {
        _streamClientFactory = streamClientFactory;
    }

    /// <summary>Generate token for the frontend.</summary>
    [HttpGet]
    public string GetToken(string userId)
    {
        var userClient = _streamClientFactory.GetUserClient();

        return userClient.CreateToken(userId);
    }
}