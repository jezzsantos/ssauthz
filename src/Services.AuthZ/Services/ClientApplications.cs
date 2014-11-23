using Services.MessageContracts;

namespace Services.AuthZ.Services
{
    partial class ClientApplications
    {
        /// <summary>
        ///     Returns the Identifier of the newly created 'ClientApplications' resource.
        /// </summary>
        protected string GetCreateClientApplicationResponseId(CreateClientApplicationResponse response)
        {
            return (response.ClientApplication != null) ? response.ClientApplication.Id : null;
        }
    }
}