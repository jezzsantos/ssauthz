using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.Configuration;
using Common.Security;
using Common.Services;
using DotNetOpenAuth.Messaging.Bindings;
using DotNetOpenAuth.OAuth2;
using DotNetOpenAuth.OAuth2.ChannelElements;
using DotNetOpenAuth.OAuth2.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services.AuthZ.Security;
using Testing.Common;

namespace Services.UnitTests.Security
{
    public class OAuthZServerSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private Mock<IClientApplicationStore> clientStore;
            private Mock<IConfigurationSettings> configuration;
            private Mock<ICryptoKeyProvider> cryptoKeyProvider;
            private Mock<ICryptoKeyStore> cryptoKeyStore;
            private OAuthZServer server;
            private Mock<IUserAuthInfoStore> userStore;

            [TestInitialize]
            public void Initialize()
            {
                configuration = new Mock<IConfigurationSettings>();
                cryptoKeyStore = new Mock<ICryptoKeyStore>();
                cryptoKeyProvider = new Mock<ICryptoKeyProvider>();
                clientStore = new Mock<IClientApplicationStore>();
                userStore = new Mock<IUserAuthInfoStore>();

                server = new OAuthZServer
                {
                    Configuration = configuration.Object,
                    CryptoKeyStore = cryptoKeyStore.Object,
                    CryptoKeyProvider = cryptoKeyProvider.Object,
                    ClientStore = clientStore.Object,
                    UserAccountStore = userStore.Object,
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenCryptoStoreAssigned()
            {
                Assert.Equal(cryptoKeyStore.Object, server.CryptoKeyStore);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenNonceStore_Throws()
            {
                Assert.Throws<NotImplementedException>(() => { INonceStore a = server.NonceStore; });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCheckAuthorizeClientCredentialsGrantWithNullRequest_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    server.CheckAuthorizeClientCredentialsGrant(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCheckAuthorizeClientCredentialsGrantWithUnknownClientApp_ReturnsNotApprovedScopes()
            {
                var scopes = new HashSet<string>(new[]
                {
                    "foo"
                });
                var request = new Mock<IAccessTokenRequest>();
                request.Setup(r => r.Scope).Returns(scopes);
                request.Setup(r => r.ClientIdentifier).Returns("bar");
                clientStore.Setup(cs => cs.GetClient(It.IsAny<string>()))
                    .Throws<ResourceNotFoundException>();

                AutomatedAuthorizationCheckResponse result = server.CheckAuthorizeClientCredentialsGrant(request.Object);

                Assert.Equal(scopes.First(), result.ApprovedScope.First());
                Assert.False(result.IsApproved);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCheckAuthorizeClientCredentialsGrantWithKnownClientApp_ReturnsApprovedScopes()
            {
                var scopes = new HashSet<string>(new[]
                {
                    "foo"
                });
                var request = new Mock<IAccessTokenRequest>();
                request.Setup(r => r.Scope).Returns(scopes);
                request.Setup(r => r.ClientIdentifier).Returns("bar");
                clientStore.Setup(cs => cs.GetClient(It.IsAny<string>()))
                    .Returns(new Mock<IClientDescription>().Object);

                AutomatedAuthorizationCheckResponse result = server.CheckAuthorizeClientCredentialsGrant(request.Object);

                Assert.Equal(scopes.First(), result.ApprovedScope.First());
                Assert.True(result.IsApproved);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCheckAuthorizeResourceOwnerCredentialGrantWithNullUserName_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    server.CheckAuthorizeResourceOwnerCredentialGrant(null, "password",
                        new Mock<IAccessTokenRequest>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCheckAuthorizeResourceOwnerCredentialGrantWithNullPassword_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    server.CheckAuthorizeResourceOwnerCredentialGrant("username", null,
                        new Mock<IAccessTokenRequest>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCheckAuthorizeResourceOwnerCredentialGrantWithNullAccessRequest_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    server.CheckAuthorizeResourceOwnerCredentialGrant("username", "password", null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCheckAuthorizeResourceOwnerCredentialGrantWithUnknownUserName_ThenReturnsFalse()
            {
                userStore.Setup(us => us.GetUserAuthInfo(It.IsAny<string>()))
                    .Returns((UserAuthInfo) null);
                var request = new Mock<IAccessTokenRequest>();
                request.Setup(r => r.Scope).Returns(new HashSet<string>(new[]
                {
                    "ascope"
                }));

                AutomatedUserAuthorizationCheckResponse result =
                    server.CheckAuthorizeResourceOwnerCredentialGrant("username", "password",
                        request.Object);

                Assert.False(result.IsApproved);
            }

            [TestMethod, TestCategory("Unit")]
            public void
                WhenCheckAuthorizeResourceOwnerCredentialGrantWithForUserAccountWithInvalidPassword_ThenReturnsFalse()
            {
                var userAccount = new UserAuthInfo
                {
                    PasswordHash = "",
                };
                userStore.Setup(us => us.GetUserAuthInfo(It.IsAny<string>()))
                    .Returns(userAccount);
                var request = new Mock<IAccessTokenRequest>();
                request.Setup(r => r.Scope).Returns(new HashSet<string>(new[]
                {
                    "ascope"
                }));

                AutomatedUserAuthorizationCheckResponse result =
                    server.CheckAuthorizeResourceOwnerCredentialGrant("username", "password",
                        request.Object);

                Assert.False(result.IsApproved);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCheckAuthorizeResourceOwnerCredentialGrantWithNullClientIdentifier_ThenReturnsFalse()
            {
                var userAccount = new UserAuthInfo
                {
                    PasswordHash = @"sha1:1000:eVtc5YWo+HlEziLNmLoMDrdY8tNr71CG:iZN6EMU5uX1aF70dfFgTGA+wNToqUsnG",
                };
                userStore.Setup(us => us.GetUserAuthInfo(It.IsAny<string>()))
                    .Returns(userAccount);
                var request = new Mock<IAccessTokenRequest>();
                request.Setup(r => r.Scope).Returns(new HashSet<string>(new[]
                {
                    "ascope"
                }));

                AutomatedUserAuthorizationCheckResponse result =
                    server.CheckAuthorizeResourceOwnerCredentialGrant("username", "password",
                        request.Object);

                Assert.False(result.IsApproved);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCheckAuthorizeResourceOwnerCredentialGrantWithWrongScope_ThenReturnsFalse()
            {
                var userAccount = new UserAuthInfo
                {
                    PasswordHash = @"sha1:1000:eVtc5YWo+HlEziLNmLoMDrdY8tNr71CG:iZN6EMU5uX1aF70dfFgTGA+wNToqUsnG",
                };
                userStore.Setup(us => us.GetUserAuthInfo(It.IsAny<string>()))
                    .Returns(userAccount);
                var request = new Mock<IAccessTokenRequest>();
                request.Setup(r => r.Scope).Returns(new HashSet<string>(new[]
                {
                    "ascope"
                }));
                request.Setup(r => r.ClientIdentifier).Returns("foo");
                clientStore.Setup(cs => cs.GetClient(It.IsAny<string>()))
                    .Returns(new Mock<IClientDescription>().Object);

                AutomatedUserAuthorizationCheckResponse result =
                    server.CheckAuthorizeResourceOwnerCredentialGrant("username", "password",
                        request.Object);

                Assert.False(result.IsApproved);
            }

            [TestMethod, TestCategory("Unit")]
            public void
                WhenCheckAuthorizeResourceOwnerCredentialGrantForUserAccountWithValidCredentialsAndScope_ThenReturnsTrue
                ()
            {
                var userAccount = new UserAuthInfo
                {
                    Roles = new[]
                    {
                        AuthorizationRoles.NormalUser
                    },
                    PasswordHash = @"sha1:1000:eVtc5YWo+HlEziLNmLoMDrdY8tNr71CG:iZN6EMU5uX1aF70dfFgTGA+wNToqUsnG",
                };
                userStore.Setup(us => us.GetUserAuthInfo(It.IsAny<string>()))
                    .Returns(userAccount);
                var request = new Mock<IAccessTokenRequest>();
                request.Setup(r => r.Scope).Returns(new HashSet<string>(new[]
                {
                    AccessScope.Profile
                }));
                request.Setup(r => r.ClientIdentifier).Returns("foo");
                clientStore.Setup(cs => cs.GetClient(It.IsAny<string>()))
                    .Returns(new Mock<IClientDescription>().Object);

                AutomatedUserAuthorizationCheckResponse result =
                    server.CheckAuthorizeResourceOwnerCredentialGrant("username", "password",
                        request.Object);

                Assert.True(result.IsApproved);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateAccessTokenWithUnknownUser_ThenReturnsAccessToken()
            {
                var request = new Mock<IAccessTokenRequest>();
                request.Setup(r => r.UserName).Returns("username");
                request.Setup(r => r.ClientIdentifier).Returns("clientid");
                cryptoKeyProvider.Setup(ckp => ckp.GetCryptoKey(It.IsAny<CryptoKeyType>()))
                    .Returns(new Mock<ICryptoKeyPair>().Object);
                server.DefaultLifetime = 1.0;

                AccessTokenResult result = server.CreateAccessToken(request.Object);

                Assert.Equal(TimeSpan.FromMinutes(1), result.AccessToken.Lifetime);
                Assert.True(result.AccessToken.UtcIssued < DateTime.Now.ToUniversalTime());
                Assert.False(result.AccessToken.ExtraData.ContainsKey(RequireAuthorizationAttribute.ExtraDataRoles));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateAccessToken_ThenReturnsAccessToken()
            {
                userStore.Setup(us => us.GetUserAuthInfo("username"))
                    .Returns(new UserAuthInfo
                    {
                        Roles = new Collection<string>(new[]
                        {
                            "foo"
                        })
                    });
                var request = new Mock<IAccessTokenRequest>();
                request.Setup(r => r.UserName).Returns("username");
                request.Setup(r => r.ClientIdentifier).Returns("clientid");
                cryptoKeyProvider.Setup(ckp => ckp.GetCryptoKey(It.IsAny<CryptoKeyType>()))
                    .Returns(new Mock<ICryptoKeyPair>().Object);
                server.DefaultLifetime = 1.0;

                AccessTokenResult result = server.CreateAccessToken(request.Object);

                Assert.Equal(TimeSpan.FromMinutes(1), result.AccessToken.Lifetime);
                Assert.True(result.AccessToken.UtcIssued < DateTime.Now.ToUniversalTime());
                Assert.Equal("foo", result.AccessToken.ExtraData[RequireAuthorizationAttribute.ExtraDataRoles]);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientWithNullClientIdentifier_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    server.GetClient(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientWithUnknownClientIdentifier_ThenThrows()
            {
                clientStore.Setup(cs => cs.GetClient(It.IsAny<string>()))
                    .Throws<ResourceNotFoundException>();

                Assert.Throws<ArgumentException>(() =>
                    server.GetClient("foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGetClientWithKnownClientIdentifier_ThenReturnsClient()
            {
                var description = new Mock<IClientDescription>();

                clientStore.Setup(cs => cs.GetClient(It.IsAny<string>()))
                    .Returns(description.Object);

                IClientDescription result = server.GetClient("foo");

                Assert.NotNull(description.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsAuthorizationValidWithNullAuthorization_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    server.IsAuthorizationValid(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsAuthorizationValidWithUnknownApplication_ThenReturnsFalse()
            {
                clientStore.Setup(cs => cs.GetClient(It.IsAny<string>()))
                    .Throws<ArgumentException>();
                var authorization = new Mock<IAuthorizationDescription>();
                authorization.Setup(az => az.User).Returns("foo");
                authorization.Setup(az => az.ClientIdentifier).Returns("bar");
                authorization.Setup(az => az.Scope).Returns(new HashSet<string>(new[]
                {
                    "ascope"
                }));

                bool result = server.IsAuthorizationValid(authorization.Object);

                Assert.False(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsAuthorizationValidWithUnknownUser_ThenReturnsFalse()
            {
                clientStore.Setup(cs => cs.GetClient(It.IsAny<string>()))
                    .Returns(new Mock<IClientDescription>().Object);
                userStore.Setup(us => us.GetUserAuthInfo(It.IsAny<string>()))
                    .Returns((UserAuthInfo) null);
                var authorization = new Mock<IAuthorizationDescription>();
                authorization.Setup(az => az.User).Returns("foo");
                authorization.Setup(az => az.ClientIdentifier).Returns("bar");
                authorization.Setup(az => az.Scope).Returns(new HashSet<string>(new[]
                {
                    "ascope"
                }));

                bool result = server.IsAuthorizationValid(authorization.Object);

                Assert.False(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsAuthorizationValidWithInvalidScope_ThenReturnsFalse()
            {
                clientStore.Setup(cs => cs.GetClient(It.IsAny<string>()))
                    .Returns(new Mock<IClientDescription>().Object);
                userStore.Setup(us => us.GetUserAuthInfo(It.IsAny<string>()))
                    .Returns(new Mock<IUserAuthInfo>().Object);
                var authorization = new Mock<IAuthorizationDescription>();
                authorization.Setup(az => az.User).Returns("foo");
                authorization.Setup(az => az.ClientIdentifier).Returns("bar");
                authorization.Setup(az => az.Scope).Returns(new HashSet<string>(new[]
                {
                    "ascope"
                }));

                bool result = server.IsAuthorizationValid(authorization.Object);

                Assert.False(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsAuthorizationValidWithKnownUserAndApplicationAndScope_ThenReturnsTrue()
            {
                clientStore.Setup(cs => cs.GetClient(It.IsAny<string>()))
                    .Returns(new Mock<IClientDescription>().Object);
                userStore.Setup(us => us.GetUserAuthInfo(It.IsAny<string>()))
                    .Returns(new Mock<IUserAuthInfo>().Object);
                var authorization = new Mock<IAuthorizationDescription>();
                authorization.Setup(az => az.User).Returns("foo");
                authorization.Setup(az => az.ClientIdentifier).Returns("bar");
                authorization.Setup(az => az.Scope).Returns(new HashSet<string>(new[]
                {
                    AccessScope.Profile
                }));

                bool result = server.IsAuthorizationValid(authorization.Object);

                Assert.True(result);
            }
        }
    }
}