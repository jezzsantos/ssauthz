using System.Collections.Generic;

namespace Common.Security
{
    /// <summary>
    ///     Defines user authentication/authorization information
    /// </summary>
    public interface IUserAuthInfo
    {
        /// <summary>
        ///     Gets or sets the user name for this account.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        ///     Gets or sets the roles for this user.
        /// </summary>
        ICollection<string> Roles { get; set; }

        /// <summary>
        ///     Gets or sets the password hash for this account.
        /// </summary>
        string PasswordHash { get; set; }

        /// <summary>
        ///     Verifies whether the password is valid.
        /// </summary>
        /// <param name="password">The password to verify</param>
        bool VerifyPassword(string password);
    }
}