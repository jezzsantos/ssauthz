using System;
using System.Web;
using Common;

namespace Services.AuthZ
{
    public class Global : HttpApplication
    {
        /// <summary>
        ///     Executes on application startup
        /// </summary>
        protected void Application_Start(object sender, EventArgs e)
        {
            Licensing.LicenseServiceStackRuntime();
            new AppHost().Init();
        }
    }
}