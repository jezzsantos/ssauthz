using System;

namespace Common.Storage.Security
{
    public interface IOAuthToken : IHasIdentifier
    {
        /// <summary>
        ///     Gets the username
        /// </summary>
        string Username { get; set; }

        /// <summary>
        ///     Gets the access token
        /// </summary>
        string AccessToken { get; set; }

        /// <summary>
        ///     Gets the refresh token
        /// </summary>
        string RefreshToken { get; set; }

        /// <summary>
        ///     Gets the date the token expires
        /// </summary>
        DateTime ExpiresOnUtc { get; set; }

        /// <summary>
        ///     Gets the date the token was issued
        /// </summary>
        DateTime IssuedOnUtc { get; set; }
    }
}