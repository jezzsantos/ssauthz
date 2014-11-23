namespace Common.Security
{
    public class ClientUserInfo : IClientUserInfo
    {
        private readonly string emailDomain;

        /// <summary>
        ///     Creates a new instance of the <see cref="ClientUserInfo" /> class.
        /// </summary>
        public ClientUserInfo(IUserAuthInfo authInfo, string password, string emailDomain)
        {
            Guard.NotNull(() => authInfo, authInfo);
            Guard.NotNullOrEmpty(() => password, password);
            Guard.NotNullOrEmpty(() => emailDomain, emailDomain);

            AuthInfo = authInfo;
            Password = password;
            this.emailDomain = emailDomain;
        }

        /// <summary>
        ///     Gets the user's authentication information
        /// </summary>
        public IUserAuthInfo AuthInfo { get; private set; }

        /// <summary>
        ///     Gets the user's password
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        ///     Gets the user's email
        /// </summary>
        public string Email
        {
            get { return AuthInfo.Username + emailDomain; }
        }
    }
}