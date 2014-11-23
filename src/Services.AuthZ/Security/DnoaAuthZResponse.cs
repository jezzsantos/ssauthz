namespace Services.AuthZ.Security
{
    /// <summary>
    ///     Defines an DNOA authorization response
    /// </summary>
    internal class DnoaAuthZResponse
    {
        /// <summary>
        ///     Gets or sets the access_token of the response
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        ///     Gets or sets the refresh_token of the response
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        ///     Gets or sets the expriy period of the response
        /// </summary>
        public string ExpiresIn { get; set; }

        /// <summary>
        ///     Gets or sets the token type of the response
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        ///     Gets or sets the scope of the response
        /// </summary>
        public string Scope { get; set; }
    }
}