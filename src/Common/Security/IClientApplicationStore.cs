using DotNetOpenAuth.OAuth2;

namespace Common.Security
{
    /// <summary>
    ///     Defines a repository of registered client applications
    /// </summary>
    public interface IClientApplicationStore
    {
        /// <summary>
        ///     Gets the client for the specific client identifier.
        /// </summary>
        IClientDescription GetClient(string clientIdentifier);
    }
}