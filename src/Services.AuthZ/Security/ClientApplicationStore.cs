using Common;
using Common.Security;
using DotNetOpenAuth.OAuth2;
using Services.AuthZ.Workflow;
using Services.DataContracts;

namespace Services.AuthZ.Security
{
    /// <summary>
    ///     The store of client applications that are registered forthis authorization server.
    /// </summary>
    internal class ClientApplicationStore : IClientApplicationStore
    {
        /// <summary>
        ///     Gets or sets the <see cref="IClientApplicationsManager" />
        /// </summary>
        public IClientApplicationsManager ClientApplicationsManager { get; set; }

        /// <summary>
        ///     Returns the specified client from the store.
        /// </summary>
        public IClientDescription GetClient(string clientIdentifier)
        {
            Guard.NotNullOrEmpty(() => clientIdentifier, clientIdentifier);

            IClientApplication clientApp =
                ClientApplicationsManager.GetClientApplicationByClientIdentifier(clientIdentifier);
            if (clientApp != null)
            {
                return new ClientDescription(clientApp.ClientSecret, null, ClientType.Confidential);
            }

            return null;
        }
    }
}