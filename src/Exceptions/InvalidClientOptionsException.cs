namespace StreamChat.Exceptions
{
    /// <summary>
    /// Thrown when invalid ClientOptions are provided.
    /// </summary>
    public class InvalidClientOptionsException : StreamBaseException
    {
        internal InvalidClientOptionsException(string message) : base(message)
        {
        }
    }
}