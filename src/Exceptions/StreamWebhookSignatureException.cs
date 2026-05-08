using System;

namespace StreamChat.Exceptions
{
    /// <summary>
    /// Thrown by <see cref="StreamChat.Clients.IAppClient.VerifyAndParseWebhook(byte[], string)"/>
    /// (and the SQS / SNS variants) when the HMAC signature on a webhook
    /// payload does not match the body the SDK was given, or when the
    /// gzip / base64 envelope is malformed.
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
