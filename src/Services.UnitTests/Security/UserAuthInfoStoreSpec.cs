using System;
using System.Linq;
using Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services.AuthZ.Security;
using Services.AuthZ.Workflow;
using Services.DataContracts;
using Testing.Common;

namespace Services.UnitTests.Security
{
    public class UserAuthInfoStoreSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private Mock<IUserAccountsManager> manager;
            private UserAuthInfoStore store;

            [TestInitialize]
            public void Initialize()
            {
                manager = new Mock<IUserAccountsManager>();
                store = new UserAuthInfoStore
                {
                    UserAccountsManager = manager.Object,
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetUserAuthInfoWithNullUserName_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () => store.GetUserAuthInfo(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetUserAuthInfoWithUnknownUserName_ThenReturnsNull()
            {
                manager.Setup(
                    uam => uam.ListUserAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Enumerable.Empty<IUserAccount>());

                IUserAuthInfo result = store.GetUserAuthInfo("foo");

                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetUserAuthInfoProfileWithKnownUserName_ThenReturnsUserAuthInfo()
            {
                var userAccount = new UserAccount
                {
                    Id = "fooid",
                    Username = "foo2",
                    PasswordHash = "bar",
                };
                manager.Setup(
                    uam => uam.ListUserAccounts(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(new[]
                    {
                        userAccount
                    });

                IUserAuthInfo result = store.GetUserAuthInfo("foo");

                Assert.Equal("bar", result.PasswordHash);
                Assert.Equal("foo2", result.Username);
            }
        }
    }
}