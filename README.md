# stream-chat-net

[![Build status](https://ci.appveyor.com/api/projects/status/ctwrjwga4gau657y/branch/master?svg=true)](https://ci.appveyor.com/project/tbarbugli/stream-chat-net/branch/master) [![NuGet Badge](https://buildstats.info/nuget/stream-chat-net)](https://www.nuget.org/packages/stream-chat-net/)

[stream-chat-net](https://github.com/GetStream/stream-chat-net) is the official .NET API client for [Stream Chat](https://getstream.io/chat), a service for building chat applications.

You can sign up for a Stream account at [https://getstream.io/chat/get_started/](https://getstream.io/chat/get_started/).

You can use this library to access chat API endpoints server-side.

For the client-side integrations (web and mobile) have a look at the Javascript, iOS and Android SDK libraries ([https://getstream.io/chat/](https://getstream.io/chat/)).

## Installation

```bash
nuget install stream-chat-net
```

## Documentation

[Official API Docs](https://getstream.io/chat/docs)

## Supported features

- Chat channels
- Messages
- Chat channel types
- User management
- Moderation API
- Push configuration
- User devices
- User search
- Channel search

## Getting started

### Import

```c#
using StreamChat;
```

### Initialize client

```c#
// snip

var client = new Client("API KEY", "API SECRET");

```

### Generate a token for clientside use

```c#
//without expiration
var token = client.CreateToken("bob-1");

//with expiration
var token = client.CreateToken("bob-1", DateTime.Now.AddHours(1));
```

### Create/Update users

```c#
var bob = new User()
{
    ID = "bob-1",
    Role = Role.Admin,
};
bob.SetData("name", "Robert Tables");
bob.SetData("teams", new string[] { "red", "blue" }; // if multi-tenant enabled

var bobFromDB = await client.Users.Upsert(bob);
Console.WriteLine(bobFromDB.CreatedAt);

//batch update is also supported
var jane = ...
var june = ...
var users = await client.Users.UpsertMany(new User[] { bob, jane, june });
```

### GDPR-like User endpoints

```c#
// exports data for one user
await client.Export("bob-1");

// deactivates a user
await client.Deactivate("bob-1");
```

### Channel types CRUD

```c#
var c = new ChannelTypeInput()
{
    Name = "livechat",
    Automod = Autmod.Disabled,
    Commands = new List<string>() { Commands.Ban },
    Mutes = true
};
//create
var chanType = await client.CreateChannelType(c);
Console.WriteLine(chanType.CreatedAt);

//update
c.Mutes = false;
chanType = await client.UpdateChannelType(chanType.Name, c);
Console.WriteLine(chanType.UpdatedAt);
Console.WriteLine(chanType.Mutes);

//get
var messaging = await client.GetChannelType("messaging");

//list
var chans = await client.ListChannelTypes();

//delete
await client.DeleteChannelType("livechat");
```

### Channels

```c#
// create a channel with members from the start
var chan = client.Channel("messaging", "bob-and-jane");
var chanFromDB = await chan.Create(bob.ID, new string[] { bob.ID, jane.ID });
chanFromDB.Members.ForEach(m => Console.WriteLine(m.User.ID));

//create channel and then add members
var chan = client.Channel("messaging", "bob-and-june");
chan.SetData("team", "red"); // if multi-tenant enabled
await chan.Create(bobFromDB.ID);
await chan.AddMembers(new string[] { bob.ID, june.ID });

//send messages
var bobHi = new MessageInput()
{
    Text = "Hi june!",
};
bobHi.SetData('location', 'amsterdam');
var juneHi = new MessageInput()
{
    Text = "Hi bob!",
};
bobHi.SetData('location', 'boulder'); // add custom data into message
var attachment = new Attachment();
attachment.SetData('x-built-by', 'android-services'); // add custom data into an attachment
bobHi.Attachments = new List<Attachment>() { attachment }; // add attachment into a message

var m1 = await chan.SendMessage(bobHi, bob.ID);
var m2 = await chan.SendMessage(juneHi, june.ID);
Console.WriteLine("{0} says {1} at {2}, {3}", m1.User.ID, m1.Text, m1.CreatedAt, m1.GetData<string>('location'));
Console.WriteLine("{0} says {1} at {2}", m2.User.ID, m2.Text, m2.CreatedAt);

//send replies
var bobReply = new MessageInput()
{
    Text = "Long time no see!"
};
var r1 = await chan.SendMessage(bobReply, bob.ID, m2.ID);
Console.WriteLine("{0} replied {1} to msg id {2}", r1.User.ID, r1.Text, r1.ParentID);

//send reactions
var juneReact = new Reaction()
{
    Type = "like"
};
var response = await chan.SendReaction(r1.ID, juneReact, june.ID);

Console.WriteLine("{0} reacted to msg id {1} with {2}", response.Reaction.User.ID, response.Message.ID, response.Reaction.Type);

//add/remove moderators
await chan.AddModerators(new string[] { june.ID });
await chan.DemoteModerators(new string[] { june.ID });

//add a ban with a timeout
await chan.BanUser(june.ID, "Being a big jerk", 4);

//removing a ban
await chan.UnbanUser(june.ID);

//query channel state
var queryParams = new ChannelQueryParams(false, true)
{
    MessagesPagination = new MessagePaginationParams() //optional
    {
        Limit = 2,
        IDLT = m1.ID
    }
};
var state = await chan.Query(queryParams);
Console.WriteLine(state.Channel.ID);
Console.WriteLine(state.Channel.Type);
state.Members.ForEach(m => Console.WriteLine("Member {0}", m.User.ID));
state.Messages.ForEach(m => Console.WriteLine("Message {0}: {1}", m.ID, m.Text));
```

### Messages

```c#
//delete a message from any channel by ID
var deletedMsg = await client.DeleteMessage(r1.ID);
//hard delete
var deletedMsg = await client.DeleteMessage(r1.ID, true);
Console.WriteLine(deletedMsg.DeletedAt);

var replies = await chan.GetReplies(m2.ID, MessagePaginationParams.Default);
Console.WriteLine(replies.Count);
```

### Devices

```c#
//add device
var junePhone = new Device()
{
    ID = "iOS Device Token",
    PushProvider = PushProvider.APN,
    UserID = june.ID
};
await client.AddDevice(junePhone);

//list devices
var devices = await client.GetDevices(june.ID);
Console.WriteLine(devices[0].ID);

//remove device
await client.RemoveDevice(junePhone.ID, junePhone.UserID);
```
