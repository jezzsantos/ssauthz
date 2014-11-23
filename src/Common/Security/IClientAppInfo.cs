using System;

namespace Common.Security
{
    /// <summary>
    ///     Defines Client Application information
    /// </summary>
    public interface IClientAppInfo
    {
        /// <summary>
        ///     Gets or sets the clientidentifier
        /// </summary>
        String ClientIdentifier { get; set; }

        /// <summary>
        ///     Gets or sets the clientsecret
        /// </summary>
        String ClientSecret { get; set; }

        /// <summary>
        ///     Gets or sets the name
        /// </summary>
        String Name { get; set; }
    }
}