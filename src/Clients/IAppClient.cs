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
        /// Reverses the encoding wrappers Stream applies to outbound webhook /
        /// SQS / SNS payloads, returning the raw JSON bytes the server signed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When <paramref name="payloadEncoding"/> is set the wrapper layer is
        /// removed first (SQS / SNS firehose envelopes wrap the bytes in base64
        /// so they remain valid UTF-8 over the queue). Then, when
        /// <paramref name="contentEncoding"/> is set, the resulting bytes are
        /// gzip-decompressed. Passing <c>null</c> (or the empty string) for
        /// either parameter is a no-op so plain HTTP webhooks behave the same
        /// as before.
        /// </para>
        /// <para>
        /// The signature Stream emits is always computed over the innermost
        /// (uncompressed, base64-decoded) JSON, so this is also the canonical
        /// representation to feed into <see cref="VerifyWebhook(string, string)"/>.
        /// </para>
        /// </remarks>
        /// <param name="body">Raw transport bytes (HTTP body, SQS <c>Body</c>, SNS <c>Message</c>).</param>
        /// <param name="contentEncoding"><c>"gzip"</c> when compression is enabled, otherwise <c>null</c>.</param>
        /// <param name="payloadEncoding"><c>"base64"</c> for SQS / SNS firehose, otherwise <c>null</c>.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when <paramref name="contentEncoding"/> or <paramref name="payloadEncoding"/>
        /// is set to a value the SDK does not support.
        /// </exception>
        /// <exception cref="StreamChat.Exceptions.StreamWebhookSignatureException">
        /// Thrown when the body fails to decode (invalid base64, invalid gzip).
        /// </exception>
        byte[] DecompressWebhookBody(byte[] body, string contentEncoding = null, string payloadEncoding = null);

        /// <summary>
        /// Decompresses (when needed) and verifies the HMAC signature, returning
        /// the uncompressed JSON bytes. The signature is always computed over
        /// the innermost (uncompressed, base64-decoded) JSON, so the verification
        /// rule is invariant across HTTP webhooks and SQS / SNS.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For HTTP webhooks: pass the raw body and the <c>X-Signature</c>
        /// header. If your app config has <c>webhook_compression_algorithm</c>
        /// set to <c>"gzip"</c> the request will arrive with
        /// <c>Content-Encoding: gzip</c> — pass that header value as
        /// <paramref name="contentEncoding"/>. (Some HTTP servers and middleware
        /// — Rails, Django, Laravel, Spring Boot, ASP.NET — strip
        /// <c>Content-Encoding</c> and decompress for you, in which case the
        /// body is already raw JSON and <paramref name="contentEncoding"/>
        /// must be left <c>null</c>.)
        /// </para>
        /// <para>
        /// For SQS / SNS firehose: pass the message body, the
        /// <c>x-signature</c> message attribute, <c>"base64"</c> for
        /// <paramref name="payloadEncoding"/>, and <c>"gzip"</c> for
        /// <paramref name="contentEncoding"/> when compression is on.
        /// </para>
        /// </remarks>
        /// <param name="body">Raw transport bytes (HTTP body, SQS <c>Body</c>, SNS <c>Message</c>).</param>
        /// <param name="signature">Lowercase hex HMAC-SHA256 signature from <c>X-Signature</c> / <c>x-signature</c>.</param>
        /// <param name="contentEncoding"><c>"gzip"</c> when compression is enabled, otherwise <c>null</c>.</param>
        /// <param name="payloadEncoding"><c>"base64"</c> for SQS / SNS firehose, otherwise <c>null</c>.</param>
        /// <exception cref="StreamChat.Exceptions.StreamWebhookSignatureException">
        /// Thrown when the signature does not match, or when the body fails to decode.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when <paramref name="contentEncoding"/> or <paramref name="payloadEncoding"/>
        /// is set to a value the SDK does not support.
        /// </exception>
        byte[] VerifyAndDecodeWebhook(byte[] body, string signature, string contentEncoding = null, string payloadEncoding = null);
    }
}