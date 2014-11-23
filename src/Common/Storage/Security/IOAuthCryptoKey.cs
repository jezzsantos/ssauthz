using System;

namespace Common.Storage.Security
{
    public interface IOAuthCryptoKey : IHasIdentifier
    {
        /// <summary>
        ///     Gets the bucket
        /// </summary>
        string Bucket { get; set; }

        /// <summary>
        ///     Gets the handle
        /// </summary>
        string Handle { get; set; }

        /// <summary>
        ///     Gets the key value
        /// </summary>
        byte[] Key { get; set; }

        /// <summary>
        ///     Gets the expiry date
        /// </summary>
        DateTime ExpiresUtc { get; set; }
    }
}