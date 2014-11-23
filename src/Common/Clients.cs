using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common.Security;

namespace Common
{
    /// <summary>
    ///     Built-in OAuth client (applications and accounts) to the ssauthz services
    /// </summary>
    public static class Clients
    {
        internal const string EmailDomain = "@mycompany.com";

        public static readonly Client Test = new Client(
            new ClientAppInfo
            {
#if TESTINGONLY
                ClientIdentifier = @"someuniqueidentifier1",
                ClientSecret = @"somesecret1",
                Name = "A Testing Application",
#endif
            },
            new ClientUserInfo(
                new UserAuthInfo
                {
#if TESTINGONLY
                    PasswordHash = PasswordHasher.CreateHash(@"somepassword"),
                    Username = @"test.user",
                    Roles = new Collection<string>(new[]
                    {
                        AuthorizationRoles.God
                    }),
#endif
                },
                @"somepassword", EmailDomain)
            );

        public static readonly Client ATrustedApplication = new Client(
            new ClientAppInfo
            {
                ClientIdentifier = @"someuniqueidentifier2",
                ClientSecret = @"somesecret",
                Name = "An Application",
            },
            new ClientUserInfo(
                new UserAuthInfo
                {
                    PasswordHash = PasswordHasher.CreateHash(@"somepassword"),
                    Username = @"an.appuser",
                    Roles = new Collection<string>(new[]
                    {
                        AuthorizationRoles.ClientApplication
                    }),
                },
                @"somepassword", EmailDomain)
            );

        /// <summary>
        ///     Gets the list of built-in clients to the services
        /// </summary>
        public static readonly IEnumerable<Client> BuiltInClients = new List<Client>(new[]
        {
            ATrustedApplication,
#if TESTINGONLY
            Test
#endif
        });

        private class ClientAppInfo : IClientAppInfo
        {
            public string ClientIdentifier { get; set; }

            public string ClientSecret { get; set; }

            public string Name { get; set; }
        }

        private class UserAuthInfo : IUserAuthInfo
        {
            public string Username { get; set; }

            public ICollection<string> Roles { get; set; }

            public string PasswordHash { get; set; }

            public bool VerifyPassword(string password)
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    ///     Defines a client that can access oAuth services.
    /// </summary>
    public class Client
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="Client" /> class.
        /// </summary>
        internal Client(IClientAppInfo clientapplication, IClientUserInfo clientUserAccount)
        {
            Guard.NotNull(() => clientapplication, clientapplication);
            Guard.NotNull(() => clientUserAccount, clientUserAccount);

            ClientApplication = clientapplication;
            ClientUserAccount = clientUserAccount;
        }

        /// <summary>
        ///     Gets or sets the client application
        /// </summary>
        public IClientAppInfo ClientApplication { get; private set; }

        /// <summary>
        ///     Gets or sets the client application user account
        /// </summary>
        public IClientUserInfo ClientUserAccount { get; private set; }
    }
}