using System;
using DotNetOpenAuth.OAuth2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services.AuthZ.Security;
using Services.AuthZ.Workflow;
using Services.DataContracts;
using Testing.Common;

namespace Services.UnitTests.Security
{
    public class ClientApplicationStoreSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private Mock<IClientApplicationsManager> manager;
            private ClientApplicationStore store;

            [TestInitialize]
            public void Initialize()
            {
                manager = new Mock<IClientApplicationsManager>();
                store = new ClientApplicationStore
                {
                    ClientApplicationsManager = manager.Object,
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientWithNullUserName_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () => store.GetClient(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientWithUnknownUserName_ThenReturnsNull()
            {
                manager.Setup(uam => uam.GetClientApplication(It.IsAny<string>()))
                    .Returns((IClientApplication) null);

                IClientDescription result = store.GetClient("foo");

                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientProfileWithKnownUserName_ThenReturnsUserAuthInfo()
            {
                var userAccount = new ClientApplication
                {
                    Id = "fooid",
                    ClientIdentifier = "foo",
                    ClientSecret = "bar",
                    Name = "bar2",
                };
                manager.Setup(uam => uam.GetClientApplicationByClientIdentifier(It.IsAny<string>()))
                    .Returns(userAccount);

                IClientDescription result = store.GetClient("foo");

                Assert.Equal(ClientType.Confidential, result.ClientType);
                Assert.Null(result.DefaultCallback);
                Assert.True(result.HasNonEmptySecret);
                Assert.True(result.IsValidClientSecret("bar"));
            }
        }
    }
}