using System;
using System.Globalization;
using System.Linq;
using System.Net;
using Common;
using Common.Security;
using Common.Services;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.DataContracts;
using Services.MessageContracts;
using Testing.Common;

namespace Services.IntTests
{
    partial class AccessTokensSpec
    {
        private const string DnoaInvalidRequestErrorMessage =
            "Error occurred while sending a direct message or getting the response";

        [TestClass]
        public class GivenDotNetOpenAuthClient : IntegrationTest
        {
            private WebServerClient authZTokenClient;


            [TestInitialize]
            public virtual void Initialize()
            {
                base.InitializeContext();
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenExchangeUserCredentialForTokenWithUnknownClientApplication_ThenThrows()
            {
                CreateAuthZTokenClient("foo", "bar");

                Assert.Throws<ProtocolException>(DnoaInvalidRequestErrorMessage,
                    () => authZTokenClient.ExchangeUserCredentialForToken("ausername", "apassword", new[]
                    {
                        AccessScope.Profile
                    }));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenExchangeUserCredentialForTokenWithUnknownUsername_ThenThrows()
            {
                CreateAuthZTokenClient(Clients.Test.ClientApplication.ClientIdentifier,
                    Clients.Test.ClientApplication.ClientSecret);

                Assert.Throws<ProtocolException>(DnoaInvalidRequestErrorMessage,
                    () => authZTokenClient.ExchangeUserCredentialForToken("ausername", "apassword", new[]
                    {
                        AccessScope.Profile
                    }));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenExchangeUserCredentialForTokenWithWrongUserPassword_ThenThrows()
            {
                CreateAuthZTokenClient(Clients.Test.ClientApplication.ClientIdentifier,
                    Clients.Test.ClientApplication.ClientSecret);

                Assert.Throws<ProtocolException>(DnoaInvalidRequestErrorMessage,
                    () => authZTokenClient.ExchangeUserCredentialForToken(
                        Clients.Test.ClientUserAccount.AuthInfo.Username, "wrongpassword", new[]
                        {
                            AccessScope.Profile
                        }));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenExchangeUserCredentialForTokenWithWrongClientApplicationSecret_ThenThrows()
            {
                CreateAuthZTokenClient(Clients.Test.ClientApplication.ClientIdentifier, "wrongsecret");

                Assert.Throws<ProtocolException>(DnoaInvalidRequestErrorMessage,
                    () => authZTokenClient.ExchangeUserCredentialForToken(
                        Clients.Test.ClientUserAccount.AuthInfo.Username,
                        Clients.Test.ClientUserAccount.Password, new[]
                        {
                            AccessScope.Profile
                        }));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenExchangeUserCredentialForTokenWithValidCredentials_ThenGetsAccessToken()
            {
                CreateAuthZTokenClient(Clients.Test.ClientApplication.ClientIdentifier,
                    Clients.Test.ClientApplication.ClientSecret);

                IAuthorizationState result = authZTokenClient.ExchangeUserCredentialForToken(
                    Clients.Test.ClientUserAccount.AuthInfo.Username,
                    Clients.Test.ClientUserAccount.Password, new[]
                    {
                        AccessScope.Profile
                    });

                Assert.True(result.AccessToken.HasValue());
                Assert.True(result.AccessTokenIssueDateUtc > DateTime.UtcNow - TimeSpan.FromSeconds(10));
                Assert.True(result.AccessTokenIssueDateUtc < DateTime.UtcNow + TimeSpan.FromSeconds(10));
                Assert.True(result.AccessTokenExpirationUtc - DateTime.UtcNow <= TimeSpan.FromMinutes(15));
                Assert.True(result.RefreshToken.HasValue());
                Assert.Equal(1, result.Scope.Count);
                Assert.Equal(AccessScope.Profile, result.Scope.First());
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenRefreshAuthorizationWithValidAccessToken_ThenGetsNewAccessToken()
            {
                CreateAuthZTokenClient(Clients.Test.ClientApplication.ClientIdentifier,
                    Clients.Test.ClientApplication.ClientSecret);

                IAuthorizationState result1 = authZTokenClient.ExchangeUserCredentialForToken(
                    Clients.Test.ClientUserAccount.AuthInfo.Username,
                    Clients.Test.ClientUserAccount.Password, new[]
                    {
                        AccessScope.Profile
                    });

                Assert.True(result1.AccessToken.HasValue());
                Assert.True(result1.AccessTokenIssueDateUtc > DateTime.UtcNow - TimeSpan.FromSeconds(10));
                Assert.True(result1.AccessTokenIssueDateUtc < DateTime.UtcNow + TimeSpan.FromSeconds(10));
                Assert.True(result1.AccessTokenExpirationUtc - DateTime.UtcNow <= TimeSpan.FromMinutes(15));
                Assert.True(result1.RefreshToken.HasValue());
                Assert.Equal(1, result1.Scope.Count);
                Assert.Equal(AccessScope.Profile, result1.Scope.First());

                bool result2 = authZTokenClient.RefreshAuthorization(result1);

                Assert.True(result2);
                Assert.True(result1.AccessToken.HasValue());
                Assert.True(result1.AccessTokenIssueDateUtc > DateTime.UtcNow - TimeSpan.FromSeconds(10));
                Assert.True(result1.AccessTokenIssueDateUtc < DateTime.UtcNow + TimeSpan.FromSeconds(10));
                Assert.True(result1.AccessTokenExpirationUtc - DateTime.UtcNow <= TimeSpan.FromMinutes(15));
                Assert.True(result1.RefreshToken.HasValue());
                Assert.Equal(1, result1.Scope.Count);
                Assert.Equal(AccessScope.Profile, result1.Scope.First());
            }

            private void CreateAuthZTokenClient(string clientIdentifier, string clientSecret)
            {
                Guard.NotNullOrEmpty(() => clientIdentifier, clientIdentifier);
                Guard.NotNullOrEmpty(() => clientSecret, clientSecret);

                authZTokenClient = new WebServerClient(new AuthorizationServerDescription
                {
                    TokenEndpoint = new Uri(Settings.GetSetting(@"AuthZService.BaseUrl.Token")),
                }, clientIdentifier, clientSecret);
            }
        }

        [TestClass]
        public class GivenTestAccounts : IntegrationTest
        {
            [TestInitialize]
            public virtual void Initialize()
            {
                base.InitializeContext();
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenPostCreateAccessTokenWithoutClientApplicationCredentials_ThenThrows()
            {
                CreateAccessToken request = MakeCreateAccessToken();

                Assert.Throws<InvalidOperationException>(HttpErrorCode.FromHttpStatusCode(HttpStatusCode.BadRequest),
                    () => Client.Post<HttpWebResponse>(request));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenPostCreateAccessTokenWithUnknownClientApplication_ThenThrows()
            {
                Client.AddCredentials("aclientapp", "asecret");

                CreateAccessToken request = MakeCreateAccessToken();
                request.Username = Clients.Test.ClientUserAccount.AuthInfo.Username;
                request.Password = Clients.Test.ClientUserAccount.Password;

                Assert.Throws<InvalidOperationException>(HttpErrorCode.FromHttpStatusCode(HttpStatusCode.BadRequest),
                    () => Client.Post<HttpWebResponse>(request));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenPostCreateAccessTokenWithWrongClientApplicationSecret_ThenThrows()
            {
                Client.AddCredentials(Clients.Test.ClientApplication.ClientIdentifier, "wrongsecret");

                CreateAccessToken request = MakeCreateAccessToken();
                request.Username = Clients.Test.ClientUserAccount.AuthInfo.Username;
                request.Password = Clients.Test.ClientUserAccount.Password;

                Assert.Throws<InvalidOperationException>(HttpErrorCode.FromHttpStatusCode(HttpStatusCode.BadRequest),
                    () => Client.Post<HttpWebResponse>(request));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenPostCreateAccessTokenWithUnknownUsername_ThenThrows()
            {
                Client.AddCredentials(Clients.Test.ClientApplication.ClientIdentifier,
                    Clients.Test.ClientApplication.ClientSecret);

                CreateAccessToken request = MakeCreateAccessToken();

                Assert.Throws<InvalidOperationException>(HttpErrorCode.FromHttpStatusCode(HttpStatusCode.BadRequest),
                    () => Client.Post<HttpWebResponse>(request));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenPostCreateAccessTokenWithWrongUserPassword_ThenThrows()
            {
                Client.AddCredentials(Clients.Test.ClientApplication.ClientIdentifier,
                    Clients.Test.ClientApplication.ClientSecret);

                CreateAccessToken request = MakeCreateAccessToken();
                request.Username = Clients.Test.ClientUserAccount.AuthInfo.Username;
                request.Password = "wrongpassword";
                Assert.Throws<InvalidOperationException>(HttpErrorCode.FromHttpStatusCode(HttpStatusCode.BadRequest),
                    () => Client.Post<HttpWebResponse>(request));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenPostCreateAccessWithValidCredentials_ThenCreatesResource()
            {
                Client.AddCredentials(Clients.Test.ClientApplication.ClientIdentifier,
                    Clients.Test.ClientApplication.ClientSecret);

                CreateAccessToken request = MakeCreateAccessToken();
                request.Username = Clients.Test.ClientUserAccount.AuthInfo.Username;
                request.Password = Clients.Test.ClientUserAccount.Password;
                CreateAccessTokenResponse result1 = Client.Post(request);

                Assert.True(result1.AccessToken.HasValue());
                Assert.Equal(TimeSpan.FromMinutes(15).TotalSeconds.ToString(CultureInfo.InvariantCulture),
                    result1.ExpiresIn);
                Assert.True(result1.RefreshToken.HasValue());
                Assert.Equal(AccessScope.Profile, result1.Scope);
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenPostCreateAccessForRefreshToken_ThenCreatesResource()
            {
                Client.AddCredentials(Clients.Test.ClientApplication.ClientIdentifier,
                    Clients.Test.ClientApplication.ClientSecret);

                //Get an access token
                CreateAccessToken request1 = MakeCreateAccessToken();
                request1.Username = Clients.Test.ClientUserAccount.AuthInfo.Username;
                request1.Password = Clients.Test.ClientUserAccount.Password;
                CreateAccessTokenResponse result1 = Client.Post(request1);

                Assert.True(result1.AccessToken.HasValue());
                Assert.Equal(TimeSpan.FromMinutes(15).TotalSeconds.ToString(CultureInfo.InvariantCulture),
                    result1.ExpiresIn);
                Assert.True(result1.RefreshToken.HasValue());
                Assert.Equal(AccessScope.Profile, result1.Scope);

                // Get a refresh token
                var request2 = new CreateAccessToken
                {
                    GrantType = GrantTypes.RefreshToken,
                    RefreshToken = result1.RefreshToken,
                };
                CreateAccessTokenResponse result2 = Client.Post(request2);

                Assert.True(result2.AccessToken.HasValue());
                Assert.Equal(TimeSpan.FromMinutes(15).TotalSeconds.ToString(CultureInfo.InvariantCulture),
                    result2.ExpiresIn);
                Assert.True(result2.RefreshToken.HasValue());
                Assert.Equal(AccessScope.Profile, result2.Scope);
            }

            private static CreateAccessToken MakeCreateAccessToken()
            {
                return new CreateAccessToken
                {
                    GrantType = GrantTypes.AccessToken,
                    Username = "ausername",
                    Password = "apassword",
                    Scope = AccessScope.Profile,
                };
            }
        }

        partial class GivenTheAccessTokensService
        {
            protected override CreateAccessToken MakeCreateAccessToken()
            {
                return new CreateAccessToken
                {
                    GrantType = GrantTypes.AccessToken,
                    Username = "ausername",
                    Password = "apassword",
                    Scope = AccessScope.Profile,
                };
            }

            [TestMethod, TestCategory("Integration")]
            public override void WhenPostCreateAccessToken_ThenCreatesResource()
            {
                Client.AddCredentials(Clients.Test.ClientApplication.ClientIdentifier,
                    Clients.Test.ClientApplication.ClientSecret);

                CreateAccessToken request = MakeCreateAccessToken();
                request.Username = Clients.Test.ClientUserAccount.AuthInfo.Username;
                request.Password = Clients.Test.ClientUserAccount.Password;
                CreateAccessTokenResponse result = Client.Post(request);

                Assert.True(result.AccessToken.HasValue());
                Assert.Equal(TimeSpan.FromMinutes(15).TotalSeconds.ToString(CultureInfo.InvariantCulture),
                    result.ExpiresIn);
                Assert.True(result.RefreshToken.HasValue());
                Assert.Equal(AccessScope.Profile, result.Scope);
            }
        }

        [TestClass]
        public class GivenUserAccounts : IntegrationTest
        {
            private UserAccount normalUser;
            private UserAccount participantUser;

            [TestInitialize]
            public virtual void Initialize()
            {
                base.InitializeContext();

                normalUser = CreateUserAccount(AuthZClient, "test.normaluser", true);
                participantUser = CreateUserAccount(AuthZClient, "test.participant", false);

                Client.AddCredentials(Clients.Test.ClientApplication.ClientIdentifier,
                    Clients.Test.ClientApplication.ClientSecret);
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenPostCreateAccessTokenForNormalUser_ThenAccessTokenReturned()
            {
                Assert.Equal("user", normalUser.Roles);

                CreateAccessToken request = MakeCreateAccessToken(normalUser);

                AssertTokenCreated(Client.Post(request));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenPostCreateAccessTokenForParticipantUser_ThenAccessTokenReturned()
            {
                Assert.Equal("participant", participantUser.Roles);

                CreateAccessToken request = MakeCreateAccessToken(participantUser);

                AssertTokenCreated(Client.Post(request));
            }

            private static void AssertTokenCreated(CreateAccessTokenResponse result)
            {
                Assert.True(result.AccessToken.HasValue());
                Assert.Equal(TimeSpan.FromMinutes(15).TotalSeconds.ToString(CultureInfo.InvariantCulture),
                    result.ExpiresIn);
                Assert.True(result.RefreshToken.HasValue());
                Assert.Equal(AccessScope.Profile, result.Scope);
            }

            private static UserAccount CreateUserAccount(ServiceClient client, string username, bool hasPassword)
            {
                // Create the account
                client.CurrentUser = Clients.Test.ClientUserAccount.AuthInfo.Username;
                CreateUserAccountResponse result = client.Post(new CreateUserAccount
                {
                    Address = new Address {Town = "atown"},
                    Email = username + "@test.com",
                    Forenames = username.Replace(".", string.Empty),
                    Surname = username.Replace(".", string.Empty),
                    Username = (hasPassword) ? username : null,
                    PasswordHash = (hasPassword) ? PasswordHasher.CreateHash("apassword") : null,
                });

                return result.UserAccount;
            }

            private static CreateAccessToken MakeCreateAccessToken(IUserAccount user)
            {
                return new CreateAccessToken
                {
                    GrantType = GrantTypes.AccessToken,
                    Username = user.Username,
                    Password = "apassword",
                    Scope = AccessScope.Profile,
                };
            }
        }
    }
}