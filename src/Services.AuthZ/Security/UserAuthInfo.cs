using System.Collections.Generic;
using Common;
using Common.Security;

namespace Services.AuthZ.Security
{
    /// <summary>
    ///     Provides user authentication/authorization information
    /// </summary>
    internal class UserAuthInfo : IUserAuthInfo
    {
        /// <summary>
        ///     Gets or sets the user name for this account.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Gets or sets the roles for this user.
        /// </summary>
        public ICollection<string> Roles { get; set; }

        /// <summary>
        ///     Gets or sets the password hash for this account.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        ///     Verifies whether the password is valid.
        /// </summary>
        /// <param name="password">The password to verify</param>
        public bool VerifyPassword(string password)
        {
            Guard.NotNullOrEmpty(() => password, password);

            return PasswordHasher.ValidatePassword(password, PasswordHash);
        }
    }
}