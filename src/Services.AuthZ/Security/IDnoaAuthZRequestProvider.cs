using Services.MessageContracts;
using ServiceStack.Web;

namespace Services.AuthZ.Security
{
    /// <summary>
    ///     Defines a DNOA authorization request provider
    /// </summary>
    internal interface IDnoaAuthZRequestProvider
    {
        /// <summary>
        ///     Handle the request to authorize the request
        /// </summary>
        /// <param name="request">The request to handle</param>
        /// <param name="jsonRequest">The parameters of the call, if JSON request</param>
        DnoaAuthZResponse HandleTokenRequest(IRequest request, CreateAccessToken jsonRequest);
    }
}