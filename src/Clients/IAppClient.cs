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
    }
}