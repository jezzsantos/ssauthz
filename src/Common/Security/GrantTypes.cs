namespace Common.Security
{
    /// <summary>
    ///     Defines the grant types for oAuth token requests
    /// </summary>
    public static class GrantTypes
    {
        public const string AccessToken = "password";
        public const string RefreshToken = "refresh_token";
    }
}