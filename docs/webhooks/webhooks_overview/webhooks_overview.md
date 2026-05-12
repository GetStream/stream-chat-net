You can listen to events using webhooks, SQS or SNS.
When setting up a webhook you can specify the exact events you want to receive, or select to receive all events.

To ensure that a webhook is triggered by Stream you can verify it's signature.
Webhook retries are in place. If you want to ensure an outage in your API never loses an event, it's better to use SQS or SNS for reliability.

## Quick Start

Here's how to quickly set up webhooks using the `event_hooks` configuration:

### Subscribe to Specific Events

```csharp
// Subscribe to message.new and message.updated events only
var webhookHook = new EventHook
{
    HookType = HookType.Webhook,
    Enabled = true,
    EventTypes = new List<string> { "message.new", "message.updated" },
    WebhookUrl = "https://example.com/webhooks/stream/messages"
};

await client.UpdateAppSettingsAsync(new AppSettingsRequest
{
    EventHooks = new List<EventHook> { webhookHook }
});
```

### Subscribe to All Events

Use an empty `event_types` array to receive all existing and future events:

```csharp
// Subscribe to all events (empty list = all events)
var webhookHook = new EventHook
{
    HookType = HookType.Webhook,
    Enabled = true,
    EventTypes = new List<string>(), // empty list = all events
    WebhookUrl = "https://example.com/webhooks/stream/all"
};

await client.UpdateAppSettingsAsync(new AppSettingsRequest
{
    EventHooks = new List<EventHook> { webhookHook }
});
```

> [!NOTE]
> For reliable event delivery, you can also configure [SQS](/chat/docs/dotnet-csharp/sqs/) or [SNS](/chat/docs/dotnet-csharp/sns/) instead of webhooks.


### Debugging webhook requests with NGROK

The easiest way to debug webhooks is with NGROK.

1. Start NGROK

```bash
brew install ngrok
ngrok http 8000
```

2. Update your webhook URL to the NGROK url

3. Trigger a webhook

4. Open up the ngrok inspector

<http://127.0.0.1:4040/inspect/http>

### Handling the webhook

A few guidelines for the webhook handling

- Webhooks should accept HTTP POST requests with JSON payloads
- Response code should be 2xx
- Webhook should be ready to accept the same call multiple times: in case of network or remote server failure Stream Chat could retry the request
- It's important to validate the signature, so you know the request originated from Stream
- Support HTTP Keep-Alive
- Use HTTPS

The example below shows how to log the message new and verify the request

```csharp
// signature comes from the HTTP header x-signature
appClient.VerifyWebhook(requestBody, signature);
```

All webhook requests contain these headers:

| Name              | Description                                                                                                          | Example                                                          |
| ----------------- | -------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------- |
| X-Webhook-Id      | Unique ID of the webhook call. This value is consistent between retries and could be used to deduplicate retry calls | 123e4567-e89b-12d3-a456-426614174000                             |
| X-Webhook-Attempt | Number of webhook request attempt starting from 1                                                                    | 1                                                                |
| X-Api-Key         | Your application’s API key. Should be used to validate request signature                                             | a1b23cdefgh4                                                     |
| X-Signature       | HMAC signature of the request body. See Signature section                                                            | ca978112ca1bbdcafac231b39a23dc4da786eff8147c4e72b9807785afee48bb |

## Compressed webhook bodies

GZIP compression can be enabled for hooks payloads from the Dashboard. Enabling compression reduces the payload size significantly (often 70–90% smaller) reducing your bandwidth usage on Stream. The computation overhead introduced by the decompression step is usually negligible and offset by the much smaller payload.

When payload compression is enabled, webhook HTTP requests will include the `Content-Encoding: gzip` header and the request body will be compressed with GZIP. Some HTTP servers and middleware (Rails, Django, Laravel, Spring Boot, ASP.NET) handle this transparently and strip the header before your handler runs — in that case the body you see is already raw JSON.

Before enabling compression, make sure that:

- Your backend integration is using a recent version of our official SDKs with compression support
- If you don't use an official SDK, make sure that your code supports receiving compressed payloads
- The payload signature check is done on the **uncompressed** payload

The .NET SDK exposes three composite helpers — `VerifyAndParseWebhook`, `VerifyAndParseSqs`, `VerifyAndParseSns` — for the HTTP, SQS, and SNS delivery channels. Each one inflates the payload when it is gzipped (detected from the body bytes per RFC 1952, independent of `Content-Encoding`), verifies the `X-Signature` HMAC against the **uncompressed** JSON, and returns the parsed `EventResponse`. The HTTP webhook path uses a constant-time comparison so the `X-Signature` header — which is exposed on a public endpoint — is not vulnerable to timing attacks; SQS/SNS deliveries arrive over AWS-internal transports where that attack vector is not applicable. All three throw `StreamInvalidWebhookException` if the signature does not match or the envelope is malformed.

The same call works whether or not Stream is currently compressing payloads for your app, so handlers do not need to change when you flip the dashboard toggle.

You can reach the helpers in three ways:

- `IStreamClientFactory.VerifyAndParseWebhook(byte[], string)` (and the `Sqs`/`Sns` variants) — convenience wrappers on the top-level factory, useful when you only need webhook verification.
- `IAppClient.VerifyAndParseWebhook(byte[], string)` (and the `Sqs`/`Sns` variants) — the same surface scoped to the app client.
- `StreamChat.Clients.WebhookHelpers.VerifyAndParseWebhook(byte[], string, string)` — a stateless static for cases where the API secret is not stored on the factory (for example, multi-tenant servers that pick the secret per request).

### ASP.NET Core handler

```csharp
[ApiController]
[Route("webhooks/stream")]
public class StreamWebhookController : ControllerBase
{
    private readonly IStreamClientFactory _stream;

    public StreamWebhookController(IStreamClientFactory stream) => _stream = stream;

    [HttpPost]
    public async Task<IActionResult> ReceiveAsync()
    {
        // Read the raw bytes off the wire — do NOT bind to a model, you need the
        // exact byte stream the server signed.
        using var ms = new MemoryStream();
        await HttpContext.Request.Body.CopyToAsync(ms);
        var rawBody = ms.ToArray();

        var signature = HttpContext.Request.Headers["X-Signature"].ToString();

        try
        {
            EventResponse ev = _stream.VerifyAndParseWebhook(rawBody, signature);
            // ...handle ev.Type, ev.Message, etc...
            return Ok();
        }
        catch (StreamInvalidWebhookException)
        {
            return Unauthorized();
        }
    }
}
```

### SQS / SNS firehose

When events are delivered through SQS or SNS the (possibly gzipped) payload is wrapped in base64 so it stays valid UTF-8 over the queue. Pass the raw message string and the `X-Signature` attribute; the SDK reverses the base64 + optional gzip wrapping in the correct order before checking the signature.

```csharp
// Inside your SQS / SNS message handler:
//   messageBody = the message body string (message.Body for SQS, the inner
//                 Message field for SNS notifications)
//   xSignature  = the value of the "X-Signature" message attribute
EventResponse ev = _stream.VerifyAndParseSqs(messageBody, xSignature);
// For SNS firehose, use VerifyAndParseSns with the SNS Message field instead:
// EventResponse ev = _stream.VerifyAndParseSns(snsMessage, xSignature);
```

## Webhook types

In addition to the above there are 3 special webhooks.

| Type                                                                       | Description                                                                                        |
| -------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------- |
| Push                                                                       | Push webhook is useful for triggering push notifications on your end                               |
| [Before Message Send](/chat/docs/dotnet-csharp/before_message_send_webhook/) | Allows you to modify or moderate message content before sending it to the chat for everyone to see |
| [Custom Commands](/chat/docs/dotnet-csharp/custom_commands_webhook/)         | Reacts to custom /slash commands                                                                   |

## Configuration

### Before Message Send and Custom Commands

These webhooks continue to use the original configuration method and are **NOT** part of the multi-event hooks system:

- **Before Message Send**: `before_message_send_hook_url`
- **Custom Commands**: `custom_action_handler_url`

```csharp
await appClient.UpdateAppSettingsAsync(new AppSettingsRequest
{
  BeforeMessageSendHookUrl = "https://example.com/webhooks/stream/before-message-send", // sets Before Message Send webhook address
  CustomActionHandlerUrl = "https://example.com/webhooks/stream/custom-commands?type={type}", // sets Custom Commands webhook address
});
```

### Push webhook

The example below shows how to use the push webhooks

```csharp
// Note: Any previously existing hooks not included in event_hooks array will be deleted.
// Get current settings first to preserve your existing configuration.

// STEP 1: Get current app settings to preserve existing hooks
var settings = await client.GetAppSettingsAsync();
var existingHooks = settings.App.EventHooks ?? new List<EventHook>();
Console.WriteLine($"Current event hooks: {existingHooks}");

// STEP 2: Add webhook hook while preserving existing hooks
var newWebhookHook = new EventHook
{
    HookType = HookType.Webhook,
    Enabled = true,
    EventTypes = new List<string>(), // empty list = all events
    WebhookUrl = "https://example.com/webhooks/stream/push"
};

// STEP 3: Update with complete array including existing hooks
var allHooks = new List<EventHook>(existingHooks) { newWebhookHook };
await client.UpdateAppSettingsAsync(new AppSettingsRequest
{
    EventHooks = allHooks
});

// Test the webhook connection
await appClient.CheckPushAsync(new AppCheckPushRequest
{
    WebhookUrl = "https://example.com/webhooks/stream/push",
});
```

You can also configure specific event types by providing an array of event names instead of an empty array:

```csharp
// Configure webhook for specific events only
var newWebhookHook = new EventHook
{
    HookType = HookType.Webhook,
    Enabled = true,
    EventTypes = new List<string> { "message.new", "message.updated", "message.deleted" }, // specific events
    WebhookUrl = "https://example.com/webhooks/stream/messages"
};
```

## Request info

Some webhooks contain a field `request_info` , which holds information about the client that issued the request. This info is intended as an additional signal that you can use for moderation, fraud detection, or other similar purposes.

When configuring the SDK, you may also set an additional `x-stream-ext` header to be sent with each request. The value of this header is passed along as an `ext` field in the `request_info` . You can use this to pass along information that may be useful, such as device information. Refer to the SDK-specific docs on how to set this header.

```json
"request_info": {
 "type": "client",
 "ip": "86.84.2.2",
 "user_agent": "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:109.0) Gecko/20100101 Firefox/117.0",
 "sdk": "stream-chat-react-10.11.0-stream-chat-javascript-client-browser-8.12.1",
 "ext": "device-id=123"
}
```

For example, in Javascript, you can set the value like this:


The format of the `ext` header is up to you and you may leave it blank if you don't need it. The value is passed as-is, so you can use a simple value, comma-separated key-values, or more structured data, such as JSON. Binary data must be encoded as a string, for example using base64 or hex encoding.

## Pending Message Options

You can configure pending message hooks to handle messages that require approval before being sent. The following options are available:

| Option        | Type   | Description                                                                | Required                             |
| ------------- | ------ | -------------------------------------------------------------------------- | ------------------------------------ |
| webhook_url   | string | The URL where pending message events will be sent                          | Yes, except for `CALLBACK_MODE_NONE` |
| timeout_ms    | number | How long messages should stay pending before being deleted in milliseconds | Yes                                  |
| callback.mode | string | Callback mode ("CALLBACK_MODE_NONE", "CALLBACK_MODE_REST")                 | Yes                                  |

You may set up to two pending message hooks per application. Only the first commit to a pending message will succeed; any subsequent commit attempts will return an error, as the message is no longer pending. If multiple hooks specify a `timeout_ms`, the system will use the longest timeout value.

For more information on configuring pending messages, please refer to the [Pending Messages](/chat/docs/dotnet-csharp/pending_messages/) documentation.

## Restricting access to webhook

If necessary, you can only expose your webhook service to Stream. This is possible by configuring your network (eg. iptables rules) to drop all incoming traffic that is not coming from our API infrastructure.

Below you can find the complete list of egress IP addresses that our webhook infrastructure uses. Such list is static and is not changing over time.

| US-East    | ZONE ID  | eip              |
| ---------- | -------- | ---------------- |
| Primary    | use1-az2 | 34.225.10.29/32  |
| Secondary  | use1-az4 | 34.198.125.61/32 |
| Tertiary   | use1-az3 | 52.22.78.160/32  |
| Quaternary | use1-az6 | 3.215.161.238/32 |

| EU-west   | ZONE ID  | eip               |
| --------- | -------- | ----------------- |
| Primary   | euw1-az3 | 52.212.14.212/32  |
| Secondary | euw1-az1 | 52.17.43.232/32   |
| Tertiary  | euw1-az2 | 34.241.110.177/32 |

| Sydney    | ZONE ID   | eip               |
| --------- | --------- | ----------------- |
| Primary   | apse2-az3 | 54.252.193.245/32 |
| Secondary | apse2-az2 | 13.55.254.141/32  |
| Tertiary  | apse2-az1 | 3.24.48.104/32    |

| mumbai    | ZONE ID  | eip              |
| --------- | -------- | ---------------- |
| Primary   | aps1-az1 | 65.1.48.87/32    |
| Secondary | aps1-az3 | 15.206.221.25/32 |
| Tertiary  | aps1-az2 | 13.233.48.78/32  |

| Singapore | ZONE ID   | eip              |
| --------- | --------- | ---------------- |
| Primary   | apse1-az2 | 13.229.11.158/32 |
| Secondary | apse1-az1 | 52.74.225.150/32 |
| Tertiary  | apse1-az3 | 52.76.180.70/32  |

| OHIO      | ZONE ID  | EIP              |
| --------- | -------- | ---------------- |
| Primary   | use2-az1 | 3.14.163.216/32  |
| Secondary | use2-az2 | 3.15.245.3/32    |
| Tertiary  | use2-az3 | 3.141.116.179/32 |

| CANADA    | ZONE ID  | EIP              |
| --------- | -------- | ---------------- |
| Primary   | cac1-az1 | 35.183.141.98/32 |
| Secondary | cac1-az2 | 52.60.71.231/32  |
| Tertiary  | cac1-az4 | 3.97.253.35/32   |

| OREGON    | ZONE ID  | EIP             |
| --------- | -------- | --------------- |
| Primary   | usw2-az1 | 52.25.165.25/32 |
| Secondary | usw2-az2 | 44.237.58.11/32 |
| Tertiary  | usw2-az3 | 52.10.213.81/32 |
