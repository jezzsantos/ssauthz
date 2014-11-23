namespace Common.Security
{
    public interface IClientUserInfo
    {
        /// <summary>
        ///     Gets the user's authentication information
        /// </summary>
        IUserAuthInfo AuthInfo { get; }

        /// <summary>
        ///     Get the user's password
        /// </summary>
        string Password { get; }

        /// <summary>
        ///     Gets the user's email
        /// </summary>
        string Email { get; }
    }
}