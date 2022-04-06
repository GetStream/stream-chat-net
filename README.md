# Official .NET SDK for [Stream Chat](https://getstream.io/chat/)

[![.github/workflows/ci.yaml](https://github.com/GetStream/stream-chat-net/actions/workflows/ci.yaml/badge.svg)](https://github.com/GetStream/stream-chat-net/actions/workflows/ci.yaml) [![NuGet Badge](https://buildstats.info/nuget/stream-chat-net)](https://www.nuget.org/packages/stream-chat-net/)

<p align="center">
    <img src="./assets/logo.svg" width="50%" height="50%">
</p>
<p align="center">
    Official .NET API client for Stream Chat, a service for building chat applications.
    <br />
    <a href="https://getstream.io/chat/docs/"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/GetStream/stream-chat-net/tree/master/samples">Code Samples</a>
    ·
    <a href="https://github.com/GetStream/stream-chat-net/issues">Report Bug</a>
    ·
    <a href="https://github.com/GetStream/stream-chat-net/issues">Request Feature</a>
</p>

## 📝 About Stream

You can sign up for a Stream account at our [Get Started](https://getstream.io/chat/get_started/) page.

You can use this library to access chat API endpoints server-side.

For the client-side integrations (web and mobile) have a look at the JavaScript, iOS and Android SDK libraries ([docs](https://getstream.io/chat/)).

---
> ## 🚨 Breaking changes in v1.0 <
> The library received many changes in v1.0 to make it easier to use and more maintanable in the future.
> The main change is that both [`Channel`](https://github.com/GetStream/stream-chat-net/blob/0.26.0/src/stream-chat-net/Channel.cs) and [`Client`](https://github.com/GetStream/stream-chat-net/blob/0.26.0/src/stream-chat-net/Client.cs) classes have been separated into small modules that we call clients. (This resambles the [structure of our Java library](https://github.com/GetStream/stream-chat-java/tree/1.5.0/src/main/java/io/getstream/chat/java/services) as well.)
> Main changes:
> - `Channel` and `Client` classes are gone, and have been organized into smaller clients in `StreamChat.Clients` namespace.
> - These clients do not maintain state as `Channel` used to did earlier where [it kept the `channelType` and `channelId` in the memory](https://github.com/GetStream/stream-chat-net/blob/0.26.0/src/stream-chat-net/Channel.cs#L34-#L35). So this means that you'll need to pass in `channelType` and `channelId` to a lot of method calls in `IChannelClient`.
> - Async method names have `Async` suffix now.
> - All public methods and classes have documentation.
> - Identifiers has been renamed from `ID` to `Id` to follow [Microsoft's naming guide](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/capitalization-conventions). Such as `userID` -> `userId`.
> - A lot of data classes have been renamed to make more sense. Such as `ChannelObject` -> `Channel`.
> - Data classes have been moved to `StreamChat.Models` namespace.
> - Full feature parity: all backend APIs are available.
> - Returned values are type of `ApiResponse` and expose rate limit informaiton with `GetRateLimit()` method.
> - The folder structure of the project has been reorganized to follow [Microsoft's recommendation](https://gist.github.com/davidfowl/ed7564297c61fe9ab814).
> - Unit tests have been improved. They are smaller, more focused and have cleanup methods.
> - Added .NET 6.0 support.
> 
> The proper usage of the library:
> ```csharp
> var clientFactory = new StreamClientFactory("YourApiKey", "YourApiSecret");
> // Note: all client instances can be used as a singleton for the lifetime
> // of your application as they don't maintain state.
> var userClient = clientFactory.GetUserClient();
> var channelClient = clientFactory.GetChannelClient();
> var messageClient = clientFactory.GetMessageClient();
> var reactionClient = clientFactory.GetReactionClient();
>
> var jamesBond = await userClient.UpsertAsync(new UserRequest { Id = "james_bond" });
> var agentM = await userClient.UpsertAsync(new UserRequest { Id = "agent_m" });
> 
> var channel = await channelClient.GetOrCreateAsync("messaging", "superHeroChannel", createdBy: jamesBond.Id);
> await channelClient.AddMembersAsync(channel.Type, channel.Id, jamesBond.Id, agentM.Id);
> 
> var message = await messageClient.SendMessageAsync(channel.Type, channel.Id, jamesBond.Id, "I need a new quest Agent M.");
> await reactionClient.SendReactionAsync(message.Id, "like", agentM.Id);
> ```

---

## ⚙️ Installation

```bash
$ dotnet add package stream-chat-net
```

> 💡 Tip: you can find code samples in the [samples](./samples) folder.

## ✨ Getting started

### Import

```c#
using StreamChat.Clients;
```

### Initialize client

```c#
// Client factory instantiation.
var clientFactory = new StreamClientFactory("YourApiKey", "YourApiSecret");

// Or you can configure some options such as custom HttpClient, HTTP timeouts etc.
var clientFactory = new StreamClientFactory("YourApiKey", "YourApiSecret", opts => opts.Timeout = TimeSpan.FromSeconds(5));

// Get clients from client factory. Note: all clients can be used as a singleton in your application.
var channelClient = clientFactory.GetChannelClient();
var messageClient = clientFactory.GetMessageClient();
```

### Generate a token for client-side usage

```c#
var userClient = clientFactory.GetUserClient();

// Without expiration
var token = userClient.CreateToken("bob-1");

// With expiration
var token = userClient.CreateToken("bob-1", expiration: DateTimeOffset.UtcNow.AddHours(1));
```

### Create/Update users

```c#
var userClient = clientFactory.GetUserClient();

var bob = new UserRequest
{
    Id = "bob-1",
    Role = Role.Admin,
    Teams = new[] { "red", "blue" } // if multi-tenant enabled
};
bob.SetData("age", 27);

await userClient.UpsertAsync(bob);

// Batch update is also supported
var jane = new UserRequest { Id = "jane"};
var june = new UserRequest { Id = "june"};
var users = await userClient.UpsertManyAsync(new[] { bob, jane, june });
```

### GDPR-like User endpoints

```c#
var userClient = clientFactory.GetUserClient();

await userClient.ExportAsync("bob-1");
await userClient.DeactivateAsync("bob-1");
await userClient.ReactivateAsync("bob-1");
await userClient.DeleteAsync("bob-1");
```

### Channel types

```c#
var channelTypeClient = clientFactory.GetChannelTypeClient();

var chanTypeConf = new ChannelTypeWithStringCommands
{
    Name = "livechat",
    Automod = Automod.Disabled,
    Commands = new List<string> { Commands.Ban },
    Mutes = true
};
var chanType = await channelTypeClient.CreateChannelTypeAsync(chanTypeConf);

var allChanTypes = await channelTypeClient.ListChannelTypesAsync();
```

### Channels

```c#
var channelClient = clientFactory.GetChannelClient();

// Create a channel with members from the start, Bob is the creator
var channel = channelClient.GetOrCreateAsync("messaging", "bob-and-jane", bob.Id, bob.Id, jane.Id);

// Create channel and then add members, Mike is the creator
var channel = channelClient.GetOrCreateAsync("messaging", "bob-and-jane", mike.Id);
channelClient.AddMembersAsync(channel.Type, channel.Id, bob.Id, jane.Id, joe.Id);
```
### Messaging
```c#
var messageClient = clientFactory.GetMessageClient();

// Only text
messageClient.SendMessageAsync(channel.Type, channel.Id, bob.Id, "Hey, I'm Bob!");

// With custom data
var msgReq = new MessageRequest { Text = "Hi june!" };
msgReq.SetData("location", "amsterdam");

var bobMessageResp = await messageClient.SendMessageAsync(channelType, channel.Id, msgReq, bob.Id);

// Threads
var juneReply = new MessageRequest { Text = "Long time no see!" };
var juneReplyMessage = await messageClient.SendMessageToThreadAsync(channel.Type, channel.Id, juneReply, june.Id, bobMessageResp.Message.Id)
```

### Reactions
```c#
var reactionClient = clientFactory.GetReactionClient();

await reactionClient.SendReactionAsync(message.Id, "like", bob.Id);

var allReactions = await reactionClient.GetReactionsAsync(message.Id);
```

### Moderation
```c#
var channelClient = clientFactory.GetChannelClient();
var userClient = clientFactory.GetUserClient();
var flagClient = clientFactory.GetFlagClient();

await channelClient.AddModeratorsAsync(channel.Type, channel.Id, new[] { jane.Id });

await userClient.BanAsync(new BanRequest
{
    Type = channel.Type,
    Id = channel.Id,
    Reason = "reason",
    TargetUserId = bob.Id,
    UserId = jane.Id
});

await flagClient.FlagUserAsync(bob.Id, jane.Id);
```
### Permissions
```c#
var permissionClient = clientFactory.GetPermissionClient();

await permissionClient.CreateRoleAsync("channel-boss");

// Assign users to roles (optional message)
await channelClient.AssignRolesAsync(new AssignRoleRequest
{
    AssignRoles = new List<RoleAssignment>
    {
        new RoleAssignment { UserId = bob.ID, ChannelRole = Role.ChannelModerator },
        new RoleAssignment { UserId = june.ID, ChannelRole = "channel-boss" }
    },
    Message = new MessageRequest { Text = "Bob and June just became mods", User = bob }
});
```

### Devices

```c#
var deviceClient = clientFactory.GetDeviceClient();

var junePhone = new Device
{
    ID = "iOS Device Token",
    PushProvider = PushProvider.APN,
    UserId = june.ID
};

await deviceClient.AddDeviceAsync(junePhone);

var devices = await deviceClient.GetDevicesAsync(june.Id);
```

### Export Channels

```c#
var channelClient = clientFactory.GetChannelClient();
var taskClient = clientFactory.GetTaskClient();

var taskResponse = channelClient.ExportChannelAsync(new ExportChannelRequest { Id = channel.Id, Type = channel.Type });

// Wait for the completion
var complete = false;
var iterations = 0;
AsyncTaskStatusResponse resp = null;
while (!complete && iterations < 1000)
{
    resp = await taskClient.GetTaskStatusAsync(taskResponse.TaskId);
    if (resp.Status == AsyncTaskStatus.Completed)
    {
        complete = true;
        break;
    }
    iterations++;
    await Task.Delay(100);
}

if (complete)
{
    Console.WriteLine(resp.Result["url"]);
}
```

## ✍️ Contributing

We welcome code changes that improve this library or fix a problem, please make sure to follow all best practices and add tests if applicable before submitting a Pull Request on Github. We are very happy to merge your code in the official repository. Make sure to sign our [Contributor License Agreement (CLA)](https://docs.google.com/forms/d/e/1FAIpQLScFKsKkAJI7mhCr7K9rEIOpqIDThrWxuvxnwUq2XkHyG154vQ/viewform) first. See our [license file](./LICENSE) for more details.

Head over to [CONTRIBUTING.md](./CONTRIBUTING.md) for some development tips.

## 🧑‍💻 We are hiring!

We've recently closed a [$38 million Series B funding round](https://techcrunch.com/2021/03/04/stream-raises-38m-as-its-chat-and-activity-feed-apis-power-communications-for-1b-users/) and we keep actively growing.
Our APIs are used by more than a billion end-users, and you'll have a chance to make a huge impact on the product within a team of the strongest engineers all over the world.

Check out our current openings and apply via [Stream's website](https://getstream.io/team/#jobs).
