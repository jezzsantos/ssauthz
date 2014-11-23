namespace Common.Security
{
    /// <summary>
    ///     Defines the supported roles
    /// </summary>
    public static class AuthorizationRoles
    {
        /// <summary>
        ///     The almighty user account
        /// </summary>
        public const string God = @"god";

        /// <summary>
        ///     A client application account
        /// </summary>
        public const string ClientApplication = @"clientapplication";

        /// <summary>
        ///     A normal user account (has credentials)
        /// </summary>
        public const string NormalUser = @"user";

        /// <summary>
        ///     A participant user account (has no credentials)
        /// </summary>
        public const string ParticipantUser = @"participant";
    }
}