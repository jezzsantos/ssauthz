using Services.DataContracts;

namespace Services.AuthZ.Workflow
{
    partial interface IClientApplicationsManager
    {
        /// <summary>
        ///     Gets the client application with the specified identifier.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IClientApplication GetClientApplication(string id);

        /// <summary>
        ///     Gets the client application with the specified clientIdentifier
        /// </summary>
        /// <param name="clientIdentifier">The clientIdentifier of the client application</param>
        /// <returns></returns>
        IClientApplication GetClientApplicationByClientIdentifier(string clientIdentifier);

        /// <summary>
        ///     Creates a new client application from the specified description.
        /// </summary>
        /// <param name="clientApplication">The client application to create</param>
        IClientApplication RegisterClientApplication(IClientApplication clientApplication);
    }
}