using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Security;
using Common.Services;
using Services.AuthZ.Workflow;
using Services.DataContracts;
using ServiceStack;

namespace Services.AuthZ.Security
{
    /// <summary>
    ///     Represents a user auth store
    /// </summary>
    internal class UserAuthInfoStore : IUserAuthInfoStore
    {
        private static ICollection<UserAuthInfo> users = new List<UserAuthInfo>();

        /// <summary>
        ///     Gets or sets the <see cref="IUserAccountsManager" />
        /// </summary>
        public IUserAccountsManager UserAccountsManager { get; set; }

        /// <summary>
        ///     Get the profile for the specified user.
        /// </summary>
        public IUserAuthInfo GetUserAuthInfo(string username)
        {
            Guard.NotNullOrEmpty(() => username, username);

            try
            {
                List<IUserAccount> accounts =
                    UserAccountsManager.ListUserAccounts("Services.AuthZ", username, null).ToList();
                if (accounts.Any())
                {
                    return accounts.First().ConvertTo<UserAuthInfo>();
                }
            }
            catch (ResourceNotFoundException)
            {
                return null;
            }

            return null;
        }
    }
}