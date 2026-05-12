using System;
using System.Threading.Tasks;
using StreamChat.Models;

namespace StreamChat.Clients
{
    /// <summary>
    /// A client that can be used to retrieve and alter application settings of a Stream Chat application.
    /// </summary>
    /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/app_setting_overview/?language=csharp</remarks>
    public interface IAppClient
    {
        /// <summary>
        /// Gets the moderation client that can be used to access moderation endpoints.
        /// </summary>
        IModerationClient Moderation { get; }

        /// <summary>
        /// <para>Returns the application settings.</para>
        /// Application level settings allow you to configure settings that impact all the channel types in your app.
        /// Our backend SDKs make it easy to change the app settings. You can also change most of these using the CLI or the dashboard.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/app_setting_overview/?language=csharp</remarks>
        Task<GetAppResponse> GetAppSettingsAsync();

        /// <summary>
        /// <para>Updates application settings.</para>
        /// Application level settings allow you to configure settings that impact all the channel types in your app.
        /// Our backend SDKs make it easy to change the app settings. You can also change most of these using the CLI or the dashboard.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/app_setting_overview/?language=csharp</remarks>
        Task<ApiResponse> UpdateAppSettingsAsync(AppSettingsRequest settings);

        /// <summary>
        /// <para>Returns rate limits.</para>
        /// Stream offers the ability to inspect an App's current rate limit quotas and usage in your App's dashboard.
        /// Alternatively you can also retrieve the API Limits for your application using the API directly.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/rate_limits/?language=csharp</remarks>
        Task<RateLimitsMap> GetRateLimitsAsync(GetRateLimitsOptions opts);

        /// <summary>
        /// Revokes all tokens issued before <paramref name="issuedBefore"/> date.
        /// </summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/tokens_and_authentication/?language=csharp#manual-token-expiration</remarks>
        Task<ApiResponse> RevokeTokensAsync(DateTimeOffset? issuedBefore);

        /// <summary>Sends a test push.</summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/push_test/?language=csharp</remarks>
        Task<AppCheckPushResponse> CheckPushAsync(AppCheckPushRequest checkPushRequest);

        /// <summary>Sends a test SQS push.</summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/push_test/?language=csharp</remarks>
        Task<AppCheckSqsResponse> CheckSqsPushAsync(AppCheckSqsRequest checkSqsRequest);

        /// <summary>Sends a test SNS push.</summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/sns/?language=csharp</remarks>
        Task<AppCheckSnsResponse> CheckSnsPushAsync(AppCheckSnsRequest checkSnsRequest);

        /// <summary>Inserts or creates a new push provider.</summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/push_introduction/?language=csharp</remarks>
        Task<UpsertPushProviderResponse> UpsertPushProviderAsync(PushProviderRequest pushProviderRequest);

        /// <summary>Lists all push providers.</summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/push_introduction/?language=csharp</remarks>
        Task<ListPushProvidersResponse> ListPushProvidersAsync();

        /// <summary>Deletes a push provider.</summary>
        /// <remarks>https://getstream.io/chat/docs/dotnet-csharp/push_introduction/?language=csharp</remarks>
        Task<ApiResponse> DeletePushProviderAsync(PushProviderType providerType, string name);

        /// <summary>Validates whether the HMAC signature is correct for the message body.</summary>
        /// <param name="requestBody">The request body to validate.</param>
        /// <param name="xSignature">The signature provided in X-Signature header.</param>
        bool VerifyWebhook(string requestBody, string xSignature);

        /// <summary>
        /// Verify and parse an HTTP webhook event.
        /// </summary>
        /// <remarks>
        /// Decompresses <paramref name="body"/> when gzipped (detected from the
        /// body bytes), verifies the <c>X-Signature</c> header against the
        /// client's API secret, and returns the parsed <see cref="EventResponse"/>.
        /// The same call works whether or not Stream is currently compressing
        /// payloads for this app, and stays correct behind middleware that
        /// auto-decompresses the request.
        /// </remarks>
        /// <param name="body">Raw HTTP request body bytes Stream signed.</param>
        /// <param name="signature">Value of the <c>X-Signature</c> header.</param>
        /// <exception cref="StreamChat.Exceptions.StreamInvalidWebhookException">
        /// Thrown when the signature does not match or the gzip envelope is malformed.
        /// </exception>
        EventResponse VerifyAndParseWebhook(byte[] body, string signature);

        /// <summary>
        /// Verify and parse an SQS firehose webhook event.
        /// </summary>
        /// <remarks>
        /// Reverses the base64 (+ optional gzip) wrapping on the SQS <c>Body</c>
        /// and returns the parsed <see cref="EventResponse"/>. Stream does not
        /// ship an <c>X-Signature</c> on SQS deliveries — those transports ride
        /// AWS-internal infrastructure (IAM-authenticated queues), so HMAC
        /// verification on top is optional. Pass <paramref name="signature"/>
        /// to opt in to verification against the client's API secret; omit it
        /// (or pass <c>null</c>) to decode-and-parse only.
        /// </remarks>
        /// <param name="messageBody">SQS message <c>Body</c> string.</param>
        /// <param name="signature">Optional <c>X-Signature</c> message attribute. When <c>null</c>, signature verification is skipped.</param>
        /// <exception cref="StreamChat.Exceptions.StreamInvalidWebhookException">
        /// Thrown when the signature does not match or the base64 / gzip envelope is malformed.
        /// </exception>
        EventResponse VerifyAndParseSqs(string messageBody, string signature = null);

        /// <summary>
        /// Verify and parse an SNS firehose webhook event.
        /// </summary>
        /// <remarks>
        /// Reverses the base64 (+ optional gzip) wrapping on the SNS notification
        /// (full envelope or pre-extracted <c>Message</c> field) and returns the
        /// parsed <see cref="EventResponse"/>. Stream does not ship an
        /// <c>X-Signature</c> on SNS deliveries — those transports ride
        /// AWS-internal infrastructure (AWS-signed SNS notifications), so HMAC
        /// verification on top is optional. Pass <paramref name="signature"/>
        /// to opt in to verification against the client's API secret; omit it
        /// (or pass <c>null</c>) to decode-and-parse only.
        /// </remarks>
        /// <param name="notificationBody">SNS HTTP POST body, or a pre-extracted <c>Message</c> field.</param>
        /// <param name="signature">Optional <c>X-Signature</c> message attribute. When <c>null</c>, signature verification is skipped.</param>
        /// <exception cref="StreamChat.Exceptions.StreamInvalidWebhookException">
        /// Thrown when the signature does not match or the base64 / gzip envelope is malformed.
        /// </exception>
        EventResponse VerifyAndParseSns(string notificationBody, string signature = null);
    }
}
