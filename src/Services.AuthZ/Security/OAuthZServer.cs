using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Common;
using Common.Configuration;
using Common.Security;
using Common.Services;
using DotNetOpenAuth.Messaging.Bindings;
using DotNetOpenAuth.OAuth2;
using DotNetOpenAuth.OAuth2.ChannelElements;
using DotNetOpenAuth.OAuth2.Messages;
using Services.AuthZ.Properties;
using ServiceStack;

namespace Services.AuthZ.Security
{
    /// <summary>
    ///     An oAuth2.0 authorization server
    /// </summary>
    internal class OAuthZServer : IAuthorizationServerHost
    {
        private double defaultLifetime;

        /// <summary>
        ///     Gets or sets the <see cref="IConfigurationSettings" />
        /// </summary>
        public IConfigurationSettings Configuration { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="IClientApplicationStore" />
        /// </summary>
        public IClientApplicationStore ClientStore { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="IUserAuthInfoStore" />
        /// </summary>
        public IUserAuthInfoStore UserAccountStore { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="ICryptoKeyProvider" />
        /// </summary>
        public ICryptoKeyProvider CryptoKeyProvider { get; set; }

        /// <summary>
        ///     Gets the default lifetime of the access token
        /// </summary>
        internal double DefaultLifetime
        {
            get
            {
                if (defaultLifetime.Equals(0))
                {
                    defaultLifetime =
                        double.Parse(
                            Configuration.GetSetting(ConfigurationSettings.AccessTokenDefaultLifetime));
                }

                return defaultLifetime;
            }
            set { defaultLifetime = value; }
        }

        /// <summary>
        ///     Gets or sets the <see cref="ICryptoKeyStore" />
        /// </summary>
        /// <remarks>
        ///     This store is for storing crypto keys used to symmetrically encrypt and sign authorization codes and refresh
        ///     tokens.
        ///     This store should be kept strictly confidential in the authorization server(s)
        ///     and NOT shared with the resource server.  Anyone with these secrets can mint
        ///     tokens to essentially grant themselves access to anything they want.
        /// </remarks>
        public ICryptoKeyStore CryptoKeyStore { get; set; }

        /// <summary>
        ///     Determines whether an access token request given a client credential grant should be authorized
        ///     and if so records an authorization entry such that subsequent calls to <see cref="IsAuthorizationValid" /> would
        ///     return <c>true</c>.
        /// </summary>
        /// <param name="accessRequest">
        ///     The access request the credentials came with.
        ///     This may be useful if the authorization server wishes to apply some policy based on the client that is making the
        ///     request.
        /// </param>
        /// <returns>A value that describes the result of the authorization check.</returns>
        /// <exception cref="NotSupportedException">
        ///     May be thrown if the authorization server does not support the client credential grant type.
        /// </exception>
        public AutomatedAuthorizationCheckResponse CheckAuthorizeClientCredentialsGrant(
            IAccessTokenRequest accessRequest)
        {
            Guard.NotNull(() => accessRequest, accessRequest);

            bool approved = IsClientExist(accessRequest.ClientIdentifier);

            var response = (new AutomatedAuthorizationCheckResponse(accessRequest, approved));

            //TODO: Audit whether the authentication passed or failed

            return response;
        }

        /// <summary>
        ///     Determines whether a given set of resource owner credentials is valid based on the authorization server's user
        ///     database
        ///     and if so records an authorization entry such that subsequent calls to <see cref="IsAuthorizationValid" /> would
        ///     return <c>true</c>.
        /// </summary>
        /// <param name="userName">Username on the account.</param>
        /// <param name="password">The user's password.</param>
        /// <param name="accessRequest">
        ///     The access request the credentials came with.
        ///     This may be useful if the authorization server wishes to apply some policy based on the client that is making the
        ///     request.
        /// </param>
        /// <returns>A value that describes the result of the authorization check.</returns>
        /// <exception cref="NotSupportedException">
        ///     May be thrown if the authorization server does not support the resource owner password credential grant type.
        /// </exception>
        public AutomatedUserAuthorizationCheckResponse CheckAuthorizeResourceOwnerCredentialGrant(string userName,
            string password, IAccessTokenRequest accessRequest)
        {
            Guard.NotNullOrEmpty(() => userName, userName);
            Guard.NotNullOrEmpty(() => password, password);
            Guard.NotNull(() => accessRequest, accessRequest);

            bool approved = false;

            //Ensure client exists
            if (IsClientExist(accessRequest.ClientIdentifier))
            {
                // Ensure user exists
                IUserAuthInfo user = UserAccountStore.GetUserAuthInfo(userName);
                if (user != null)
                {
                    if (IsValidScope(accessRequest))
                    {
                        if (user.VerifyPassword(password))
                        {
                            approved = true;

                            //TODO: audit the passed authentication
                        }

                        //TODO: audit the failed authentication
                    }
                }
            }

            return (new AutomatedUserAuthorizationCheckResponse(accessRequest, approved, (approved) ? userName : null));
        }

        /// <summary>
        ///     Acquires the access token and related parameters that go into the formulation of the token endpoint's response to a
        ///     client.
        /// </summary>
        /// <param name="accessTokenRequestMessage">
        ///     Details regarding the resources that the access token will grant access to, and the identity of the client
        ///     that will receive that access.
        ///     Based on this information the receiving resource server can be determined and the lifetime of the access
        ///     token can be set based on the sensitivity of the resources.
        /// </param>
        /// <returns>A non-null parameters instance that DotNetOpenAuth will dispose after it has been used.</returns>
        public AccessTokenResult CreateAccessToken(IAccessTokenRequest accessTokenRequestMessage)
        {
            TimeSpan clientApplicationLifetime = GetClientLifetime(accessTokenRequestMessage);

            var accessToken = new AuthorizationServerAccessToken
            {
                // Note: all other fields are assigned by IsAuthorizationValid() (i.e. ClientIdentifier, Scope, User and UtcIssued)

                // Set the crypto keys for accessing the secured services (assume there is only one secured service)
                AccessTokenSigningKey =
                    CryptoKeyProvider.GetCryptoKey(CryptoKeyType.AuthZServer).PrivateEncryptionKey,
                ResourceServerEncryptionKey = GetRequestedSecureResourceCryptoKey(),

                // Set the limited lifetime of the token
                Lifetime = (clientApplicationLifetime != TimeSpan.Zero)
                    ? clientApplicationLifetime
                    : TimeSpan.FromMinutes(DefaultLifetime),
            };

            // Insert user specific information
            string username = GetUserFromAccessTokenRequest(accessTokenRequestMessage);
            if (username.HasValue())
            {
                IUserAuthInfo user = GetUserAuthInfo(username);
                if (user != null)
                {
                    accessToken.ExtraData.Add(new KeyValuePair<string, string>(
                        RequireAuthorizationAttribute.ExtraDataRoles, String.Join(@",", user.Roles)));
                }
            }

            return new AccessTokenResult(accessToken);
        }

        /// <summary>
        ///     Determines whether a described authorization is (still) valid.
        /// </summary>
        /// <param name="authorization">The authorization.</param>
        /// <returns>
        ///     <c>true</c> if the original authorization is still valid; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         When establishing that an authorization is still valid,
        ///         it's very important to only match on recorded authorizations that
        ///         meet these criteria:
        ///     </para>
        ///     1) The client identifier matches.
        ///     2) The user account matches.
        ///     3) The scope on the recorded authorization must include all scopes in the given authorization.
        ///     4) The date the recorded authorization was issued must be <em>no later</em> that the date the given authorization
        ///     was issued.
        ///     <para>
        ///         One possible scenario is where the user authorized a client, later revoked authorization,
        ///         and even later reinstated authorization.  This subsequent recorded authorization
        ///         would not satisfy requirement #4 in the above list.  This is important because the revocation
        ///         the user went through should invalidate all previously issued tokens as a matter of
        ///         security in the event the user was revoking access in order to sever authorization on a stolen
        ///         account or piece of hardware in which the tokens were stored.
        ///     </para>
        /// </remarks>
        public bool IsAuthorizationValid(IAuthorizationDescription authorization)
        {
            Guard.NotNull(() => authorization, authorization);

            // Verify the user still exists
            if (IsUserExists(authorization.User))
            {
                //Verify the application still exists
                if (IsClientExist(authorization.ClientIdentifier))
                {
                    //Verify the scope is valid
                    if ((authorization.Scope.Count == 1)
                        && (authorization.Scope.First().EqualsIgnoreCase(AccessScope.Profile)))
                    {
                        return true;
                    }
                }
            }

            //TODO: When we support user configuration of applications and scopes of access
            //TODO: Verify the scopes the user permits
            //TODO: verify that the user still permits this application
            //TODO: Verify the date issued is no sooner than the date the user permitted the application

            // TODO: This is where we should check (e.g. in our authorization database) 
            //that the resource owner has not revoked the authorization associated with the request.
            return (false);
        }

        /// <summary>
        ///     Gets the client with a given identifier.
        /// </summary>
        /// <param name="clientIdentifier">The client identifier.</param>
        /// <returns>The client registration.  Never null.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when no client with the given identifier is registered with this
        ///     authorization server.
        /// </exception>
        public IClientDescription GetClient(string clientIdentifier)
        {
            Guard.NotNullOrEmpty(() => clientIdentifier, clientIdentifier);

            try
            {
                // Lookup client in store
                return ClientStore.GetClient(clientIdentifier);
            }
            catch (ResourceNotFoundException)
            {
                throw new ArgumentException(
                    Resources.OAuthZServer_UnknownClientIdentifier.FormatWithInvariant(clientIdentifier));
            }
        }

        /// <summary>
        ///     Gets the authorization code nonce store to use to ensure that authorization codes can only be used once.
        /// </summary>
        /// <value>The authorization code nonce store.</value>
        public INonceStore NonceStore
        {
            get { throw new NotImplementedException(); }
        }

        private static TimeSpan GetClientLifetime(IAccessTokenRequest accessTokenRequestMessage)
        {
            // TODO: a registered user (username) may have only granted access to 
            //the requesting app (ClientIdentifier) for a specific period.
            // We would need to shorten the default token as much

            return TimeSpan.Zero;
        }

        private RSACryptoServiceProvider GetRequestedSecureResourceCryptoKey()
        {
            return CryptoKeyProvider.GetCryptoKey(CryptoKeyType.ApiService).PublicSigningKey;
        }

        private bool IsClientExist(string clientIdentifier)
        {
            if (!clientIdentifier.HasValue())
            {
                return false;
            }

            try
            {
                return (GetClient(clientIdentifier) != null);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private bool IsUserExists(string username)
        {
            if (!username.HasValue())
            {
                return false;
            }

            return (GetUserAuthInfo(username) != null);
        }

        private IUserAuthInfo GetUserAuthInfo(string username)
        {
            return UserAccountStore.GetUserAuthInfo(username);
        }

        /// <summary>
        ///     Extratcs the name of the user from the request
        /// </summary>
        /// <remarks>
        ///     There is a bug in DNOA where the <see cref="IAccessTokenRequest.UserName" /> is null when requesting a
        ///     refreshtoken.
        ///     Thsi method works around it by finding the username in the request.
        /// </remarks>
        private static string GetUserFromAccessTokenRequest(IAccessTokenRequest accessTokenRequest)
        {
            if (accessTokenRequest.UserName.HasValue())
            {
                return accessTokenRequest.UserName;
            }

            var request = accessTokenRequest as IAuthorizationDescription;
            if (request != null)
            {
                return request.User;
            }

            if (accessTokenRequest is AccessTokenRequestBase)
            {
                // Use reflection to get username
                Type type = accessTokenRequest.GetType();
                return (from p in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                    where typeof (IAuthorizationDescription).IsAssignableFrom(p.PropertyType)
                    select ((IAuthorizationDescription) p.GetValue(accessTokenRequest)).User).FirstOrDefault();
            }

            return null;
        }

        private static bool IsValidScope(IAccessTokenRequest accessRequest)
        {
            return (accessRequest.Scope.Count == 1)
                   && (accessRequest.Scope.First().EqualsIgnoreCase(AccessScope.Profile));
        }
    }
}