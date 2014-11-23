using ServiceStack;

namespace Services.MessageContracts
{
#if TESTINGONLY
    /// <summary>
    /// A web role reset response
    /// </summary>
    public class ResetWebRoleResponse
    {
        /// <summary>
        /// Gets or sets the <see cref="ServiceStack.ResponseStatus"/> of the response.
        /// </summary>
        public ResponseStatus ResponseStatus { get; set; }
    }
#endif
}
