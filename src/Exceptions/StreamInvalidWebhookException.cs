using System;

namespace StreamChat.Exceptions
{
    /// <summary>
    /// Unified exception for every webhook verification and parsing failure
    /// surfaced by <see cref="StreamChat.Clients.IAppClient.VerifyAndParseWebhook(byte[], string)"/>,
    /// <see cref="StreamChat.Clients.IAppClient.ParseSqs(string)"/>,
    /// <see cref="StreamChat.Clients.IAppClient.ParseSns(string)"/>,
    /// the equivalent helpers on
    /// <see cref="StreamChat.Clients.IStreamClientFactory"/>, and the stateless
    /// primitives in <see cref="StreamChat.Clients.WebhookHelpers"/>. A single
    /// exception type is thrown for every failure mode so handlers only need
    /// one <c>catch</c> arm; the specific cause is identified by the message
    /// constants exposed on this class.
    /// </summary>
    /// <remarks>
    /// Covers all webhook envelope and content failures: HMAC signature
    /// mismatch (<see cref="SignatureMismatch"/>), corrupt gzip
    /// (<see cref="GzipFailed"/>), invalid base64 in the SQS / SNS envelope
    /// (<see cref="InvalidBase64"/>), malformed JSON
    /// (<see cref="InvalidJson"/>), unparseable SNS envelopes, and any other
    /// schema or structural defect detected before the parsed
    /// <see cref="StreamChat.Models.EventResponse"/> is returned. Callers can
    /// switch on <see cref="Exception.Message"/> when mode-specific behaviour
    /// is required.
    /// </remarks>
#if !NETCORE
    [Serializable]
#endif
    public class StreamInvalidWebhookException : StreamBaseException
    {
        public const string SignatureMismatch = "signature mismatch";
        public const string InvalidBase64 = "invalid base64 encoding";
        public const string GzipFailed = "gzip decompression failed";
        public const string InvalidJson = "invalid JSON payload";

        public StreamInvalidWebhookException(string message) : base(message)
        {
        }

        public StreamInvalidWebhookException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
