using System.Net;
using ServiceStack;

namespace Services.MessageContracts
{
#if TESTINGONLY
    /// <summary>
    /// Request a reset of the web role
    /// </summary>
    [Route(@"/tests/resetwebrole", @"GET",
        Summary = @"Request a reset of the web role",
        Notes = @"This endpoint is for integration testing only, and should only be visible in a TESTINGONLY build.")]
    [Api(Description = @"Testing Only")]
    [ApiResponse(HttpStatusCode.OK, "The request succeeded")]
    [ApiResponse(HttpStatusCode.InternalServerError,
        @"An unexpected error occured in the service while performing this operation")]
    [ApiResponse(HttpStatusCode.Unauthorized,
        @"You are not authorized to complete this request. Either you don't have the rights to access this resource for this request, or you have not provided the required Bearer token in the request to be identifed for this request"
        )]
    public class ResetWebRole : IReturn<ResetWebRoleResponse>
    {
    }
#endif
}
