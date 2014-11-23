using System;

namespace Common.Storage.Security
{
    public class OAuthCryptoKey : IOAuthCryptoKey
    {
        /// <summary>
        ///     Gets the ID of the key
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Gets the bucket
        /// </summary>
        public string Bucket { get; set; }

        /// <summary>
        ///     Gets the handle
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        ///     Gets the key value
        /// </summary>
        public byte[] Key { get; set; }

        /// <summary>
        ///     Gets the expiry date
        /// </summary>
        public DateTime ExpiresUtc { get; set; }
    }
}