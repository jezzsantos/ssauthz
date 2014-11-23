using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;

namespace Services.IntTests
{
    /// <summary>
    /// A simple Json client that uses oAuth Bearer tokens
    /// </summary>
    public class ServiceClient : JsonServiceClient
    {
        public ServiceClient(string baseUrl) : base(baseUrl)
        {
        }

        public void AddCredentials(object clientIdentifier, object clientSecret)
        {
        }

        public string CurrentUser { get; set; }
    }
}
