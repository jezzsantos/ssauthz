namespace Common.Security
{
    /// <summary>
    ///     Defines a user auth store
    /// </summary>
    public interface IUserAuthInfoStore
    {
        /// <summary>
        ///     Gets the auth info for the specified username.
        /// </summary>
        IUserAuthInfo GetUserAuthInfo(string username);
    }
}