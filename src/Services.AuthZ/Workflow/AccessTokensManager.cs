using Common;
using Common.Services.Workflow;
using Services.AuthZ.Security;
using Services.MessageContracts;
using ServiceStack.Web;

namespace Services.AuthZ.Workflow
{
    internal class AccessTokensManager : WorkflowManager, IAccessTokensManager
    {
        /// <summary>
        ///     Gets or sets the <see cref="IDnoaAuthZRequestProvider" />
        /// </summary>
        public IDnoaAuthZRequestProvider DnoaAuthorizationServer { get; set; }

        CreateAccessTokenResponse IAccessTokensManager.CreateAccessToken(IRequest request, CreateAccessToken body)
        {
            return CreateAccessToken(request, body);
        }

        /// <summary>
        ///     Creates a new access token for a specific user.
        /// </summary>
        internal CreateAccessTokenResponse CreateAccessToken(IRequest request, CreateAccessToken body)
        {
            Guard.NotNull(() => request, request);
            Guard.NotNull(() => body, body);

            // Delegate to DNOA to process the incoming request
            DnoaAuthZResponse response = DnoaAuthorizationServer.HandleTokenRequest(request, body);

            var accessToken = new CreateAccessTokenResponse
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                ExpiresIn = response.ExpiresIn,
                TokenType = response.TokenType,
                Scope = response.Scope,
            };

            //TODO: Audit the creation of the access_token

            return accessToken;
        }
    }
}