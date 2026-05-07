using System;

namespace StreamChat.Exceptions
{
    /// <summary>
    /// Thrown by <see cref="StreamChat.Clients.IAppClient.VerifyAndDecodeWebhook(byte[], string, string, string)"/>
    /// when the HMAC signature on a webhook / SQS / SNS payload does not
    /// match the body the SDK was given.
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
