using ServiceStack;

namespace Services.DataContracts
{
    /// <summary>
    ///     Extensions to the <see cref="UserAccount" /> class
    /// </summary>
    public static class UserAccountExtensions
    {
        /// <summary>
        ///     Returns the full name of the account
        /// </summary>
        public static string GetFullName(this UserAccount account)
        {
            return @"{0} {1}".FormatWith(account.Forenames, account.Surname).Trim();
        }

        /// <summary>
        ///     Returns the origin of the account
        /// </summary>
        public static string GetOrigin(this UserAccount account)
        {
            return @"{0}, {1}".FormatWith(account.Address.Town).Trim();
        }

        /// <summary>
        ///     Returns the full email of the account (in the form: Email, FullName)
        /// </summary>
        public static string GetFullEmail(this UserAccount account)
        {
            return @"{0}, {1}".FormatWith(account.Email, account.GetFullName()).Trim();
        }
    }
}