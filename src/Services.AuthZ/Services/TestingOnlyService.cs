using System.Net;
using Common.Services;
using Services.MessageContracts;
using ServiceStack;

#if TESTINGONLY

namespace Services.AuthZ.Services
{
    /// <summary>
    /// A simple smoke test class
    /// </summary>
    internal class TestingOnlyService : ServiceBase, ITestingOnlyService
    {

        /// <summary>
        /// Resets the web role
        /// </summary>
        public ResetWebRoleResponse Get(ResetWebRole request)
        {
            // Clear existing instance
            if (ServiceStackHost.Instance != null)
            {
                ServiceStackHost.Instance.Dispose();
            }

            // Initialize a new instance of the service
            new AppHost().Init();

            this.Response.StatusCode = (int)HttpStatusCode.OK;
            this.Response.End();

            return null;
        }
    }
}

#endif
