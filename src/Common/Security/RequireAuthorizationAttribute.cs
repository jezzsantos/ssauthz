using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Common.Services;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth2;
using ServiceStack;
using ServiceStack.Web;

namespace Common.Security
{
    /// <summary>
    ///     An attribute to enforce oAuth2.0 authorization, for the specified scope.
    /// </summary>
    /// <remarks>
    ///     When this attribute is placed on any service verb or message contract it ensures that the current request contains
    ///     an oAuth2.0 bearer token issued for the user from the Authorization server.
    ///     If the token is invalid or expired, the client is returned HTTP 401.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class RequireAuthorizationAttribute : RequestFilterAttribute
    {
        public const string ExtraDataRoles = @"roles";

        private static readonly char[] RoleDelimiters =
        {
            ' ',
            ',',
            ';'
        };

        private readonly string[] scopes;

        /// <summary>
        ///     Creates a new instance of the <see cref="RequireAuthorizationAttribute" /> class.
        /// </summary>
        /// <param name="scopes"></param>
        public RequireAuthorizationAttribute(params string[] scopes)
        {
            this.scopes = scopes;

            // Set default scopes
            if (scopes == null)
            {
                scopes = new[]
                {
                    AccessScope.Profile
                };
            }
            Priority = -100;
        }

        /// <summary>
        ///     Executes the filter
        /// </summary>
        [DebuggerStepThrough]
        public override void Execute(IRequest request, IResponse response, object requestDto)
        {
            try
            {
                var cryptoKeyProvider = ServiceStackHost.Instance.Container.Resolve<ICryptoKeyProvider>();
                ICryptoKeyPair authZServerKeys = cryptoKeyProvider.GetCryptoKey(CryptoKeyType.AuthZServer);
                ICryptoKeyPair apiServiceKeys = cryptoKeyProvider.GetCryptoKey(CryptoKeyType.ApiService);

                var tokenAnalyzer = new StandardAccessTokenAnalyzer(authZServerKeys.PublicSigningKey,
                    apiServiceKeys.PrivateEncryptionKey);
                var resourceServer = new ResourceServer(tokenAnalyzer);

                // Verify the signed bearer token (for specified scopes), and extract the user's identity
                AccessToken token = resourceServer.GetAccessToken((HttpRequestBase) request.OriginalRequest, scopes);

                // Assign this user to the current HTTP context
                var user = new OAuthPrincipal(token.User, GetUserRoles(token));
                ((HttpRequestBase) request.OriginalRequest).RequestContext.HttpContext.User = user;
            }
            catch (ProtocolFaultResponseException ex)
            {
                //TODO: if the token is invalid in some way (i.e. malformed) then return 400-BadRequest.
                // The token is either: expired or invalid or revoked, we need to return 401-Unauthorized
                response.AddHeader(HttpHeaders.WwwAuthenticate, @"Bearer");
                throw HttpErrorThrower.Unauthorized(ex.Message);
            }
        }

        private static string[] GetUserRoles(AccessToken token)
        {
            var roles = new List<string>();
            if (token.ExtraData.ContainsKey(ExtraDataRoles))
            {
                roles = token.ExtraData[ExtraDataRoles]
                    .Split(RoleDelimiters, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            return roles.ToArray();
        }
    }
}