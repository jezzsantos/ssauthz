using System;

namespace Common.Storage.Security
{
    internal class OAuthToken : IOAuthToken
    {
        /// <summary>
        ///     Gets the ID of the user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Gets the username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Gets the access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        ///     Gets the refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        ///     Gets the date the token expires
        /// </summary>
        public DateTime ExpiresOnUtc { get; set; }

        /// <summary>
        ///     Gets the date the token was issued
        /// </summary>
        public DateTime IssuedOnUtc { get; set; }
    }
}