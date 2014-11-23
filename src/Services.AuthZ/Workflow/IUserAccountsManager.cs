using System.Collections.Generic;
using Services.DataContracts;

namespace Services.AuthZ.Workflow
{
    partial interface IUserAccountsManager
    {
        /// <summary>
        ///     Gets the account for the specified identifier.
        /// </summary>
        IUserAccount GetUserAccount(string currentUsername, string id);

        /// <summary>
        ///     Gets the account for the specified username.
        /// </summary>
        IEnumerable<IUserAccount> ListUserAccounts(string currentUsername, string username, string email);

        /// <summary>
        ///     Creates a new account with the specified details.
        /// </summary>
        IUserAccount RegisterUserAccount(string currentUsername, IUserAccount userAccount);
    }
}