using System;

namespace StreamChat.Exceptions
{
    /// <summary>
    /// Base exception for all exceptions that are thrown by the SDK
    /// </summary>
#if !NETCORE
    [Serializable]
#endif
    public abstract class StreamBaseException : Exception
    {
        internal StreamBaseException(string message) : base(message)
        {
        }

        internal StreamBaseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}