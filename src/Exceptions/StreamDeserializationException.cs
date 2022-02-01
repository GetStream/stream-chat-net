using System;

namespace StreamChat.Exceptions
{
    public class StreamDeserializationException : StreamBaseException
    {
        internal StreamDeserializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}