using System.Net;
using ServiceStack;
using ServiceStack.Text;

namespace Common.Services
{
    /// <summary>
    ///     Defines extensions to the <see cref="AppHostBase" /> class
    /// </summary>
    public static class AppHostExtensions
    {
        /// <summary>
        ///     Adds a global request filter that prevent HTTP access (i.e. HTTPS only).
        /// </summary>
        public static void PreventUnsecuredHttpAccess(this AppHostBase appHost)
        {
            appHost.GlobalRequestFilters.AddIfNotExists((req, res, dto) =>
            {
                // Ensure we have an HTTPS request
                if (!req.IsSecureConnection)
                {
                    res.StatusCode = (int) HttpStatusCode.Forbidden;
                    res.Close();
                }
            });
        }

        /// <summary>
        ///     Configures the JSON configuration for the service
        /// </summary>
        public static void SetJsonConfig(this AppHostBase appHost)
        {
            // Assume all dates are UTC dates
            JsConfig.DateHandler = DateHandler.ISO8601;
            JsConfig.AssumeUtc = true;
            JsConfig.AlwaysUseUtc = true;
        }
    }
}