## Authentication

Stream uses JWT (JSON Web Tokens) to authenticate users so they can open WebSocket connections and send API requests. When a user opens your app, they first pass through your own authentication system. After that, the Stream SDK is initialized and a client instance is created. The device then requests a Stream token from your server. Your server verifies the request and returns a valid token. Once the device receives this token, the user is authenticated and ready to start using chat.

## Generating Tokens

You can generate tokens on the server by creating a Server Client and then using the Create Token method.

If generating a token to use client-side, the token must include the userID claim in the token payload, whereas server tokens do not. When using the create token method, pass the user_id parameter to generate a client-side token.

```csharp
// nuget install stream-chat-net

using StreamChat.Clients;

// Instantiate your Stream client factory using the API key and secret
// the secret is only used server side and gives you full access to the API.

var factory = new StreamClientFactory("{{ api_key }}", "{{ api_secret }}");
var userClient = factory.GetUserClient();

var token = userClient.CreateToken("john");
```

### Manually Generating Tokens

You can use the JWT generator on this page to generate a User Token JWT without needing to set up a server client. You can use this token for prototyping and debugging; usually by hardcoding this into your application or passing it as an environment value at initialization.

You will need the following values to generate a token:

- `User ID` : A unique string to identify a user.

- `API Secret` : You can find this value in the [Dashboard](https://getstream.io/dashboard/).

To generate a token, provide a `User ID` and your `API Secret` to the following generator:

_Use the [Stream Dashboard](https://getstream.io/dashboard/) to generate tokens for testing._

For more information on how JWT works, please visit <https://jwt.io>.

## Setting Automatic Token Expiration

By default, user tokens are valid indefinitely. You can set an expiration to tokens by passing it as the second parameter. The expiration should contain the number of seconds since Unix epoch (00:00:00 UTC on 1 January 1970).

```csharp
// creates a token valid for 1 hour
var token = userClient.CreateToken("john", DateTimeOffset.UtcNow.AddHours(1));
```

## Token Providers

A concept we will refer to throughout the docs is a Token Provider. At a high level, the Token Provider is an endpoint on your server that can perform the following sequence of tasks:

1. Receive information about a user from the front end.

2. Validate that user information against your system.

3. Provide a User-ID corresponding to that user to the server client's token creation method.

4. Return that token to the front end.

To learn more about Token Providers, read on in our [Initialization & Users](/chat/docs/dotnet-csharp/init_and_users/) section.

## Developer Tokens

For development applications, it is possible to disable token authentication and use client-side generated tokens or a manually generated static token. Disabling auth checks is not suitable for a production application and should only be done for proofs-of-concept and applications in the early development stage. To enable development tokens, you need to change your application configuration.

On the [Dashboard](https://getstream.io/dashboard/):

1. Select the App you want to enable developer tokens on and ensure it is in Development mode

2. Click _App_ name to enter the _Chat Overview_

3. Scroll to the _General_ section

4. Toggle _Disable Authentication Checks_

This disables the authentication check, but does not remove the requirement to send a token. Send either a client generated development token, or manually create one and hard code it into your application.

```csharp
var userName = "The Amazing Tom";
var userId = StreamChatClient.SanitizeUserId(userName); // Remove disallowed characters
var userToken = StreamChatClient.CreateDeveloperAuthToken(userId);
var credentials = new AuthCredentials("API_KEY", userId, userToken);

// Create chat client
var client = StreamChatClient.CreateDefaultClient();

// Connect user
var localUserData = await client.ConnectUserAsync(credentials);
```

## Manual Token Expiration

Token Revocation is a way to manually expire tokens for a single user or for many users by setting a `revoke_tokens_issued_before` time, and any tokens issued before this will be considered expired and will fail to authenticate.  This can be reversed by setting the field to null.

### Token Revocation by User

You can revoke all tokens that belong to a certain user or a list of users.

```csharp
var issuedBefore = DateTimeOffset.UtcNow.AddHours(-1);

await userClient.RevokeUserTokenAsync("<user-id>", issuedBefore);

await userClient.RevokeManyUserTokensAsync(new[] { user1.Id, user2.Id }, issuedBefore);
```

Note: Your tokens must include the `iat` (issued at time) claim, which will be compared to the time in the `revoke_tokens_issued_before` field to determine whether the token is valid or expired.  Tokens which have no `iat` will be considered invalid.

### Undoing the revoke

To undo user-level token revocation, you can simply set revocation date to `null`:

```csharp
await userClient.RevokeUserTokenAsync("<user-id>", null);

await userClient.RevokeManyUserTokensAsync(new[] { user1.Id, user2.Id }, null);
```

### Token Revocation by Application

It is possible to revoke tokens for all users of an application. This should be used with caution as it will expire every user's token, regardless of whether the token has an `iat` claim.

```csharp
await appClient.RevokeTokensAsync(DateTimeOffset.UtcNow.AddHours(-1));
```

### Undoing the revoke

To undo app-level token revocation, you can simply set revocation date to `null`:

```csharp
await appClient.RevokeTokensAsync(null);
```

### Adding iat claim to token

By default, user tokens generated through the createToken function do not contain information about time of issue. You can change that by passing the issue date as the third parameter while creating tokens. This is a security best practice, as it enables revoking tokens.

```csharp
var token = userClient.CreateToken(user1.Id,
        expiration: DateTimeOffset.UtcNow.AddHours(2),
        issuedAt: DateTimeOffset.UtcNow);
```
