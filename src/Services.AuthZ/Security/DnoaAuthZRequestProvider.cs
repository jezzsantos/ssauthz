using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using Common.Services;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using Services.MessageContracts;
using ServiceStack;
using ServiceStack.Web;

namespace Services.AuthZ.Security
{
    /// <summary>
    ///     Provides a DNOA authorization request provider
    /// </summary>
    internal class DnoaAuthZRequestProvider : IDnoaAuthZRequestProvider
    {
        /// <summary>
        ///     Gets or sets the <see cref="IAuthorizationServerHost" />
        /// </summary>
        public IAuthorizationServerHost OAuthorizationServer { get; set; }

        /// <summary>
        ///     Handle the request to authorize the request
        /// </summary>
        /// <param name="request">The request to handle</param>
        /// <param name="jsonRequest">The parameters of the call, if JSON request</param>
        public DnoaAuthZResponse HandleTokenRequest(IRequest request, CreateAccessToken jsonRequest)
        {
            OutgoingWebResponse response = GetResponse(request, jsonRequest);

            var responseBody = response.Body.FromJson<Dictionary<string, string>>();
            if (response.Status == HttpStatusCode.OK)
            {
                return new DnoaAuthZResponse
                {
                    AccessToken = responseBody[@"access_token"],
                    RefreshToken = responseBody[@"refresh_token"],
                    ExpiresIn = responseBody[@"expires_in"],
                    TokenType = responseBody[@"token_type"],
                    Scope = responseBody[@"scope"],
                };
            }

            string error = (responseBody.ContainsKey(@"error")) ? responseBody[@"error"] : string.Empty;
            string message = (responseBody.ContainsKey(@"error_description"))
                ? responseBody[@"error_description"]
                : error;
            throw LogicErrorThrower.RuleViolation(message);
        }

        private OutgoingWebResponse GetResponse(IRequest request, CreateAccessToken jsonRequest)
        {
            var authZServer = new AuthorizationServer(OAuthorizationServer);

            if (request.ContentType == MimeTypes.Json)
            {
                // Request is coming from A JSON client
                return authZServer.HandleTokenRequest(ConvertJsonRequestToFormRequest(request, jsonRequest));
            }
            // Request is likely coming from DNOA client
            return authZServer.HandleTokenRequest((HttpRequestBase) request.OriginalRequest);
        }

        /// <summary>
        ///     Populates the request object form fields from fields in body
        /// </summary>
        /// <remarks>
        ///     The DotNetOpenAuth library expects a <see cref="MimeTypes.FormUrlEncoded" /> request that contains all the
        ///     necessary form fields,
        ///     (equivalent to the fields in the JSON body).
        ///     When we call this REST method from the DotNetOpenAuth client, the form fields are populated,
        ///     AND ServiceStack reads them into the fields of the JSON body for us.
        ///     When we call this REST method from a JsonClient however, the form fields are not populated from the fields of the
        ///     JSON body.
        ///     This needs to be done manually before the DotNetOpenAuth methods will complete properly.
        /// </remarks>
        private static HttpRequestMessage ConvertJsonRequestToFormRequest(IRequest request,
            CreateAccessToken jsonRequest)
        {
            var requestMsg = new HttpRequestMessage(HttpMethod.Post, request.AbsoluteUri);

            var fields = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(@"grant_type", jsonRequest.GrantType),
                new KeyValuePair<string, string>(@"username", jsonRequest.Username),
                new KeyValuePair<string, string>(@"password", jsonRequest.Password),
                new KeyValuePair<string, string>(@"refresh_token", jsonRequest.RefreshToken),
                new KeyValuePair<string, string>(@"scope", jsonRequest.Scope),
            };

            // Fill in form content
            requestMsg.Content = new FormUrlEncodedContent(fields);

            //Copy any headers from original request (except ContentType and ContentLength)
            request.Headers
                .ToDictionary()
                .ForEach((name, value) =>
                {
                    if (!name.Equals(HttpHeaders.ContentType)
                        && !name.Equals(HttpHeaders.ContentLength))
                    {
                        requestMsg.Headers.Add(name, value);
                    }
                });

            // Set caching header
            requestMsg.Headers.Add(HttpHeaders.CacheControl, new[]
            {
                "no-store",
                "no-cache"
            });

            return requestMsg;
        }
    }
}