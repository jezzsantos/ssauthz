using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common;
using Common.Reflection;
using Common.Security;
using Common.Services;
using Common.Services.Workflow;
using Common.Storage.DataEntities;
using Services.AuthZ.Properties;
using Services.DataContracts;
using Services.MessageContracts;
using ServiceStack;
using ServiceStack.Web;

namespace Services.AuthZ.Workflow
{
    /// <summary>
    ///     The manager for user accounts
    /// </summary>
    internal class UserAccountsManager : WorkflowManager<IUserAccount>, IUserAccountsManager
    {
        GetUserAccountResponse IUserAccountsManager.GetUserAccount(IRequest request, GetUserAccount body)
        {
            return new GetUserAccountResponse
            {
                UserAccount = (UserAccount) GetUserAccount(request.GetCurrentUser(), body.Id),
            };
        }

        ListUserAccountsResponse IUserAccountsManager.ListUserAccounts(IRequest request, ListUserAccounts body)
        {
            return new ListUserAccountsResponse
            {
                UserAccounts =
                    ListUserAccounts(request.GetCurrentUser(), body.Username, body.Email)
                        .Cast<UserAccount>()
                        .ToList(),
            };
        }

        CreateUserAccountResponse IUserAccountsManager.CreateUserAccount(IRequest request, CreateUserAccount body)
        {
            return new CreateUserAccountResponse
            {
                UserAccount =
                    (UserAccount)
                        CreateUserAccount(request.GetCurrentUser(), body.Username, body.PasswordHash,
                            body.Forenames, body.Surname, body.Email, body.MobilePhone, body.Address, null),
            };
        }

        UpdateUserAccountResponse IUserAccountsManager.UpdateUserAccount(IRequest request, UpdateUserAccount body)
        {
            return new UpdateUserAccountResponse
            {
                UserAccount =
                    (UserAccount)
                        UpdateUserAccount(request.GetCurrentUser(), body.Id, body.OldPasswordHash,
                            body.NewPasswordHash, body.Forenames,
                            body.Surname, body.Email, body.MobilePhone, body.Address),
            };
        }

        DeleteUserAccountResponse IUserAccountsManager.DeleteUserAccount(IRequest request, DeleteUserAccount body)
        {
            DeleteUserAccount(request.GetCurrentUser(), body.Id);
            return new DeleteUserAccountResponse();
        }

        public IUserAccount GetUserAccount(string currentUsername, string id)
        {
            Guard.NotNull(() => currentUsername, currentUsername);
            Guard.NotNull(() => id, id);

            // Get the account
            IUserAccount account = Storage.Get(id);
            if (account == null)
            {
                throw LogicErrorThrower.ResourceNotFound();
            }

            return account;
        }

        public IUserAccount RegisterUserAccount(string currentUsername, IUserAccount userAccount)
        {
            Guard.NotNull(() => currentUsername, currentUsername);
            Guard.NotNull(() => userAccount, userAccount);

            // Verify account does not already exist
            IEnumerable<IUserAccount> existingAccounts = Storage.Find(Storage.BuildQuery(
                Reflector<IUserAccount>.GetPropertyName(x => x.Username), QueryOperator.EQ, userAccount.Username));
            if (!existingAccounts.Any())
            {
                return CreateUserAccount(currentUsername, userAccount.Username, userAccount.PasswordHash,
                    userAccount.Forenames, userAccount.Surname, userAccount.Email, userAccount.MobilePhone,
                    userAccount.Address,
                    userAccount.Roles);
            }

            return null;
        }

        public IEnumerable<IUserAccount> ListUserAccounts(string currentUsername, string username, string email)
        {
            Guard.NotNull(() => currentUsername, currentUsername);

            if (username.HasValue())
            {
                return Storage.Find(Storage.BuildQuery(
                    Reflector<UserAccount>.GetPropertyName(x => x.Username), QueryOperator.EQ, username));
            }

            if (email.HasValue())
            {
                return Storage.Find(Storage.BuildQuery(
                    Reflector<UserAccount>.GetPropertyName(x => x.Email), QueryOperator.EQ, email));
            }

            return Enumerable.Empty<IUserAccount>();
        }

        internal IUserAccount CreateUserAccount(string currentUsername, string username, string passwordHash,
            string forenames, string surname, string email, string mobilePhone, Address address, string roles = null)
        {
            Guard.NotNull(() => currentUsername, currentUsername);
            Guard.NotNullOrEmpty(() => forenames, forenames);
            Guard.NotNullOrEmpty(() => surname, surname);
            Guard.NotNullOrEmpty(() => email, email);
            Guard.NotNull(() => address, address);

            // Check email not already registered
            if (FindUserAccount(x => x.Email, email))
            {
                throw LogicErrorThrower.ResourceConflict(Resources.UserAccountsManager_UserAccountExistsByEmail, email);
            }

            // Check username not already used
            if (CredentialsProvided(username, passwordHash) && FindUserAccount(x => x.Username, username))
            {
                throw LogicErrorThrower.ResourceConflict(Resources.UserAccountsManager_UserAccountExistsByUsername,
                    username);
            }

            // Can't specify role if no credentials
            if (!CredentialsProvided(username, passwordHash) && roles.HasValue())
            {
                throw new RuleViolationException(Resources.UserAccountsManager_NoRolesForParticipant);
            }

            var newAccount = new UserAccount
            {
                Username = (CredentialsProvided(username, passwordHash)) ? username : email,
                PasswordHash = (CredentialsProvided(username, passwordHash)) ? passwordHash : null,
                Forenames = forenames,
                Surname = surname,
                MobilePhone = mobilePhone,
                Address = address,
                Email = email,
                Roles = CalculateRoles(username, passwordHash, roles),
                IsRegistered = CredentialsProvided(username, passwordHash),
            };

            string accountId = Storage.Add(newAccount);
            newAccount.Id = accountId;

            //TODO: Audit the creation of the user account

            return newAccount;
        }

        internal IUserAccount UpdateUserAccount(string currentUsername, string id, string oldPasswordHash,
            string newPasswordHash,
            string forename, string surname, string email, string mobilePhone, Address address)
        {
            Guard.NotNull(() => currentUsername, currentUsername);
            Guard.NotNull(() => id, id);

            // Get the account
            IUserAccount accountToUpdate = Storage.Get(id);
            if (accountToUpdate == null)
            {
                throw LogicErrorThrower.ResourceNotFound();
            }
            accountToUpdate.Address = accountToUpdate.Address ?? new Address();

            //Verify password hashes
            if (newPasswordHash.HasValue())
            {
                if (!accountToUpdate.PasswordHash.EqualsOrdinal(oldPasswordHash))
                {
                    throw LogicErrorThrower.RuleViolation(Resources.UserAccountsManager_PasswordsDontMatch);
                }

                accountToUpdate.PasswordHash = newPasswordHash;
            }

            //Update (allowable) account properties
            accountToUpdate.Forenames = forename;
            accountToUpdate.Surname = surname;
            accountToUpdate.Email = email;
            accountToUpdate.MobilePhone = mobilePhone;
            if (address != null)
            {
                accountToUpdate.Address.PopulateWithNonDefaultValues(address);
            }

            IUserAccount newAccount = Storage.Update(id, accountToUpdate);

            // TODO: Audit the update of the account

            return newAccount;
        }

        internal void DeleteUserAccount(string currentUsername, string id)
        {
            Guard.NotNull(() => currentUsername, currentUsername);
            Guard.NotNull(() => id, id);

            // Get the account
            IUserAccount account = Storage.Get(id);
            if (account == null)
            {
                throw LogicErrorThrower.ResourceNotFound();
            }

            // Delete the account
            Storage.Delete(id);

            //TODO: Audit the deletion of the user account
        }

        private bool FindUserAccount(Expression<Func<IUserAccount, string>> propertyName, string value)
        {
            IEnumerable<IUserAccount> existingAccounts = Storage.Find(
                Storage.BuildQuery(
                    Reflector<IUserAccount>.GetPropertyName(propertyName), QueryOperator.EQ, value));

            return (existingAccounts.Any());
        }

        private static bool CredentialsProvided(string username, string passwordHash)
        {
            return (username.HasValue()
                    && passwordHash.HasValue());
        }

        private static string CalculateRoles(string username, string passwordHash, string roles)
        {
            if (CredentialsProvided(username, passwordHash))
            {
                return (!roles.HasValue() ? AuthorizationRoles.NormalUser : roles);
            }

            return AuthorizationRoles.ParticipantUser;
        }
    }
}