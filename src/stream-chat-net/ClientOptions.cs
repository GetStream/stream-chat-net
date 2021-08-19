using System;

namespace StreamChat
{
    public class ClientOptions
    {
        public static ClientOptions Default = new ClientOptions();

        /// <summary>
        /// Number of milliseconds to wait on requests
        /// </summary>
        /// <remarks>Default is 3000</remarks>
        public int Timeout { get; set; }

        public ClientOptions()
        {
            Timeout = 3000;
        }
    }
}
