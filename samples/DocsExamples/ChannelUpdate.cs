using StreamChat.Clients;
using StreamChat.Models;

namespace DocsExamples;

/// <summary>
/// Code examples for <see href="https://getstream.io/chat/docs/node/channel_update/"/>
/// </summary>
internal class ChannelUpdate
{
    private readonly IChannelClient _channelClient;

    public ChannelUpdate()
    {
        var factory = new StreamClientFactory("{{ api_key }}", "{{ api_secret }}");
        _channelClient = factory.GetChannelClient();
    }

    public async Task ArchivingAChannel()
    {
        // Archive
        var archiveResponse = await _channelClient.ArchiveAsync("messaging", "channel-id", "user-id");

        // Get the date when the channel got archived by the user
        var archivedAt = archiveResponse.ChannelMember.ArchivedAt;

        // Get channels that are NOT archived
        var unarchivedChannels = await _channelClient.QueryChannelsAsync(new QueryChannelsOptions
        {
            Filter = new Dictionary<string, object>
            {
                { "archived", false },
            },
            UserId = "user-id",
        });

        // Unarchive
        var unarchiveResponse = await _channelClient.UnarchiveAsync("messaging", "channel-id", "user-id");
    }

    public async Task PinningAChannel()
    {
        // Pin
        var pinResponse = await _channelClient.PinAsync("messaging", "channel-id", "user-id");

        // Get the date when the channel got pinned by the user
        var pinnedAt = pinResponse.ChannelMember.PinnedAt;

        // Get channels pinned for the user
        var pinnedChannels = await _channelClient.QueryChannelsAsync(new QueryChannelsOptions
        {
            Filter = new Dictionary<string, object>()
            {
                { "pinned", true },
            },
            UserId = "user-id",
        });

        // Unpin
        var unpinResponse = await _channelClient.UnpinAsync("messaging", "channel-id", "user-id");
    }
}