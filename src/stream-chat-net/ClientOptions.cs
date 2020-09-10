using System;

namespace StreamChat
{
    public enum ApiLocation
    {
        Dublin,
        USEast,
        Singapore,
        Sydney
    }

    public class ClientOptions
    {
        public static ClientOptions Default = new ClientOptions();

        /// <summary>
        /// Number of milliseconds to wait on requests
        /// </summary>
        /// <remarks>Default is 3000</remarks>
        public int Timeout { get; set; }

        public ApiLocation Location { get; set; }

        public ClientOptions()
        {
            Location = ApiLocation.USEast;
            Timeout = 3000;
        }
    }
}
