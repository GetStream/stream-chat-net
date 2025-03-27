using StreamChat.Clients;
using StreamChat.Models;

var streamClientFactory = new StreamClientFactory(
    Environment.GetEnvironmentVariable("STREAM_KEY"),
    Environment.GetEnvironmentVariable("STREAM_SECRET"));

var userClient = streamClientFactory.GetUserClient();
var channelClient = streamClientFactory.GetChannelClient();
var msgClient = streamClientFactory.GetMessageClient();

var jamesBond = new UserRequest { Id = "james_bond" };
await userClient.UpsertAsync(jamesBond);

var agentM = new UserRequest { Id = "agent_m" };
await userClient.UpsertAsync(agentM);

var channelResp = await channelClient.GetOrCreateAsync("messaging", "my-channel", createdBy: jamesBond.Id, members: new[] { jamesBond.Id, agentM.Id });
var channel = channelResp.Channel;

await msgClient.SendMessageAsync(channel.Type, channel.Id, agentM.Id, "Hello, James Bond!");
Console.WriteLine($"Message sent to {channel.Cid}!");
