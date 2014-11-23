using Services.MessageContracts;

namespace Services.AuthZ.Services
{
    partial class UserAccounts
    {
        /// <summary>
        ///     Returns the Identifier of the newly created 'UserAccounts' resource.
        /// </summary>
        protected string GetCreateUserAccountResponseId(CreateUserAccountResponse response)
        {
            return (response.UserAccount != null) ? response.UserAccount.Id : null;
        }
    }
}