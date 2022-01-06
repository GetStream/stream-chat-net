namespace StreamChat.Clients
{
    /// <summary>
    /// Client for generating a JWT (Json web token),
    /// </summary>
    internal interface IJwtGeneratorClient
    {
        /// <summary>
        /// Generate a JWT (Json web token) for server side usage.
        /// 'server' claim must be in the payload.
        /// </summary>
        string GenerateServerSideJwt(string apiSecret);

        /// <summary>
        /// Generate a JWT (Json web token) for the given user.
        /// 'user_id claim must be in the payload.
        /// </summary>
        string GenerateJwt(object payload, string apiSecret);
    }
}