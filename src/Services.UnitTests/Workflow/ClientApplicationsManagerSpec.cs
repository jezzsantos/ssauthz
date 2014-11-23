using System;
using System.Linq;
using Common;
using Common.Services;
using Common.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services.AuthZ.Properties;
using Services.AuthZ.Workflow;
using Services.DataContracts;
using Testing.Common;

namespace Services.UnitTests.Workflow
{
    public class ClientApplicationsManagerSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private ClientApplicationsManager clientManager;
            private Mock<IStorageProvider<IClientApplication>> storageProvider;

            [TestInitialize]
            public void Initialize()
            {
                storageProvider = new Mock<IStorageProvider<IClientApplication>>();
                clientManager = new ClientApplicationsManager
                {
                    Storage = storageProvider.Object,
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientApplicationWithNullId_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () => clientManager.GetClientApplication(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientApplicationWithUnknownId_ThenThrowsNotFound()
            {
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>())).Returns((IClientApplication) null);

                Assert.Throws<ResourceNotFoundException>(
                    () => clientManager.GetClientApplication("foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientApplicationWithKnownId_ThenReturnsApp()
            {
                var app = new Mock<IClientApplication>();
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>())).Returns(app.Object);

                IClientApplication result = clientManager.GetClientApplication("foo");

                Assert.Equal(app.Object, result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientApplicationByClientIdentifierWithNullId_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () => clientManager.GetClientApplicationByClientIdentifier(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientApplicationByClientIdentifierWithUnknownId_ThenThrowsNotFound()
            {
                storageProvider.Setup(sp => sp.Find(It.IsAny<string>()))
                    .Returns(Enumerable.Empty<IClientApplication>());

                Assert.Throws<ResourceNotFoundException>(
                    () => clientManager.GetClientApplicationByClientIdentifier("foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientApplicationByClientIdentifierWithKnownId_ThenReturnsApp()
            {
                var app = new Mock<IClientApplication>();
                storageProvider.Setup(sp => sp.Find(It.IsAny<string>())).Returns(new[]
                {
                    app.Object
                });

                IClientApplication result = clientManager.GetClientApplicationByClientIdentifier("foo");

                Assert.Equal(app.Object, result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateClientApplicationWithNullName_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() => clientManager.CreateClientApplication(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateClientApplicationWithExistingName_ThenThrows()
            {
                var app = new Mock<IClientApplication>();
                storageProvider.Setup(sp => sp.Find(It.IsAny<string>())).Returns(new[]
                {
                    app.Object
                });

                Assert.Throws<ResourceConflictException>(
                    Resources.ClientApplicationsManager_ApplicationByNameExists,
                    () => clientManager.CreateClientApplication("foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateClientApplicationWithNewName_ThenCreatesNewApp()
            {
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>())).Returns((IClientApplication) null);

                IClientApplication result = clientManager.CreateClientApplication("foo");

                storageProvider.Verify(sp => sp.Add(It.IsAny<IClientApplication>()), Times.Once());
                Assert.NotNull(result);
                Assert.Equal("foo", result.Name);
                Assert.False(!result.ClientIdentifier.HasValue());
                Assert.False(!result.ClientSecret.HasValue());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeleteClientApplicationWithNullId_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() => clientManager.DeleteClientApplication(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeleteClientApplicationWithUnknownId_ThenThrowsNotFound()
            {
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>())).Returns((IClientApplication) null);

                Assert.Throws<ResourceNotFoundException>(() => clientManager.DeleteClientApplication("foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeleteClientApplicationWithKnownId_ThenReturnsApp()
            {
                var app = new Mock<IClientApplication>();
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>())).Returns(app.Object);

                clientManager.DeleteClientApplication("foo");

                storageProvider.Verify(sp => sp.Delete("foo"), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenRegisterClientApplicationWithNullName_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() => clientManager.RegisterClientApplication(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenRegisterClientApplicationWithExistingIdentifier_ThenReturnsNull()
            {
                var app = new Mock<IClientApplication>();
                storageProvider.Setup(sp => sp.Find(It.IsAny<string>())).Returns(new[]
                {
                    app.Object
                });

                IClientApplication result = clientManager.RegisterClientApplication(new ClientApplication());

                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenRegisterClientApplicationWithNewApplication_ThenRegistersNewApp()
            {
                storageProvider.Setup(sp => sp.Get(It.IsAny<string>())).Returns((IClientApplication) null);

                IClientApplication result = clientManager.RegisterClientApplication(new ClientApplication
                {
                    Name = "foo",
                    ClientIdentifier = "bar",
                    ClientSecret = "bar2",
                });

                storageProvider.Verify(sp => sp.Add(It.IsAny<IClientApplication>()), Times.Once());
                Assert.NotNull(result);
                Assert.Equal(@"foo", result.Name);
                Assert.Equal(@"bar", result.ClientIdentifier);
                Assert.Equal(@"bar2", result.ClientSecret);
            }
        }
    }
}