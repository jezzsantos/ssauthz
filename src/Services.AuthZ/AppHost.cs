using System.Linq;
using Common;
using Common.Configuration;
using Common.Security;
using Common.Services;
using Common.Storage;
using Common.Storage.Security;
using DotNetOpenAuth.Messaging.Bindings;
using DotNetOpenAuth.OAuth2;
using Funq;
using Services.AuthZ.Security;
using Services.AuthZ.Workflow;
using Services.DataContracts;
using ServiceStack;
using ServiceStack.Api.Swagger;

namespace Services.AuthZ
{
    /// <summary>
    ///     A host for all services.
    /// </summary>
    partial class AppHost
    {
        /// <summary>
        ///     Configures this <see cref="AppHostBase" />.
        /// </summary>
        public override void Configure(Container container)
        {
            base.Configure(container);
            Plugins.Add(new SwaggerFeatureWrapper(new SwaggerFeature(), @"docs"));

#if DEBUG
            SetConfig(new HostConfig
            {
                DebugMode = true,
            });
#endif
            // Create the initial Applications
            RegisterClientApplications();
        }

        /// <summary>
        ///     Registers other custom types
        /// </summary>
        protected override void RegisterCustomTypes(Container container)
        {
            container.RegisterAutoWiredAs<AppConfigurationSettings, IConfigurationSettings>();
            container.RegisterAutoWiredAs<OAuthCryptoKeyStorageProvider, IStorageProvider<IOAuthCryptoKey>>();
            container.RegisterAutoWiredAs<CryptoKeyStore, ICryptoKeyStore>();
            container.RegisterAutoWiredAs<ClientApplicationStore, IClientApplicationStore>();
            container.RegisterAutoWiredAs<UserAuthInfoStore, IUserAuthInfoStore>();
            container.Register<ICryptoKeyProvider>(
                x => new CryptoKeyProvider(ConfigurationSettings.CryptoKeyHelperCertificate)
                {
                    Configuration = x.Resolve<IConfigurationSettings>(),
                });
            container.RegisterAutoWiredAs<OAuthZServer, IAuthorizationServerHost>();
            container.RegisterAutoWiredAs<DnoaAuthZRequestProvider, IDnoaAuthZRequestProvider>();
        }

        /// <summary>
        ///     Creates the initial registrations for the known oAuth client applications
        /// </summary>
        private static void RegisterClientApplications()
        {
            var clientAppManager = Instance.Container.Resolve<IClientApplicationsManager>();
            var userAccountManager = Instance.Container.Resolve<IUserAccountsManager>();

            // Register the built in ClientApps & ClientApp Users
            Clients.BuiltInClients.ToList().ForEach(bic =>
            {
                clientAppManager.RegisterClientApplication(bic.ClientApplication.ConvertTo<ClientApplication>());
                userAccountManager.RegisterUserAccount(@"Services.AuthZ", CreateUserAccount(bic.ClientUserAccount));
            });
        }

        private static UserAccount CreateUserAccount(IClientUserInfo userInfo)
        {
            var account = userInfo.AuthInfo.ConvertTo<UserAccount>();
            account.Forenames = account.Username;
            account.Forenames = userInfo.AuthInfo.Username;
            account.Surname = userInfo.AuthInfo.Username;
            account.Email = userInfo.Email;
            account.MobilePhone = string.Empty;
            account.Address = new Address();
            account.Roles = string.Join(", ", userInfo.AuthInfo.Roles);

            return account;
        }
    }
}