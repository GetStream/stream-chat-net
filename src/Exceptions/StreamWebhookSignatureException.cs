using System;

namespace StreamChat.Exceptions
{
    /// <summary>
    /// Thrown by <see cref="StreamChat.Clients.IAppClient.VerifyAndParseWebhook(byte[], string)"/>,
    /// the SQS / SNS variants, and the equivalent helpers on
    /// <see cref="StreamChat.Clients.IStreamClientFactory"/> and
    /// <see cref="StreamChat.Clients.WebhookHelpers"/>. Surfaced when the HMAC
    /// signature on a webhook payload does not match the body the SDK was given,
    /// when the gzip / base64 envelope is malformed, or when the JSON inside the
    /// envelope cannot be parsed into an <see cref="StreamChat.Models.EventResponse"/>.
    /// </summary>
#if !NETCORE
    [Serializable]
#endif
    public class StreamWebhookSignatureException : StreamBaseException
    {
        internal StreamWebhookSignatureException(string message) : base(message)
        {
        }

        internal StreamWebhookSignatureException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
