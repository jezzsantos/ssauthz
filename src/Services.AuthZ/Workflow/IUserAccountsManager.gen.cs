﻿//------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Services.MessageContracts;
using ServiceStack.Web;

namespace Services.AuthZ.Workflow
{
    /// <summary>
    /// Defines the workflow manager for processing the 'UserAccounts' resource.
    /// </summary>
    internal partial interface IUserAccountsManager
    {
        /// <summary>
        /// Processes the GetUserAccount request for the 'UserAccounts' resource.
        /// </summary>
        GetUserAccountResponse GetUserAccount(IRequest request, GetUserAccount body);

        /// <summary>
        /// Processes the ListUserAccounts request for the 'UserAccounts' resource.
        /// </summary>
        ListUserAccountsResponse ListUserAccounts(IRequest request, ListUserAccounts body);

        /// <summary>
        /// Processes the CreateUserAccount request for the 'UserAccounts' resource.
        /// </summary>
        CreateUserAccountResponse CreateUserAccount(IRequest request, CreateUserAccount body);

        /// <summary>
        /// Processes the UpdateUserAccount request for the 'UserAccounts' resource.
        /// </summary>
        UpdateUserAccountResponse UpdateUserAccount(IRequest request, UpdateUserAccount body);

        /// <summary>
        /// Processes the DeleteUserAccount request for the 'UserAccounts' resource.
        /// </summary>
        DeleteUserAccountResponse DeleteUserAccount(IRequest request, DeleteUserAccount body);

    }
}