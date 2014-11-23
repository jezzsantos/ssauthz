using System.Net;
using Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.MessageContracts;
using Testing.Common;

namespace Services.IntTests
{
    partial class ClientApplicationsSpec
    {
        partial class GivenTheClientApplicationsService
        {
            protected override CreateClientApplication MakeCreateClientApplication()
            {
                var request = base.MakeCreateClientApplication();
                request.Name = "foo";

                return request;
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenPostCreateClientApplicationWithSameApplication_ThenThrowsExisting()
            {
                // Create first application
                Client.Post(MakeCreateClientApplication());

                // Create same application
                Assert.Throws<ResourceConflictException>(HttpErrorCode.FromHttpStatusCode(HttpStatusCode.Conflict),
                    () =>
                        Client.Post(MakeCreateClientApplication()));
            }
        }
    }
}
