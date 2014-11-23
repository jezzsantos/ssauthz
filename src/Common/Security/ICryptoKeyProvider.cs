namespace Common.Security
{
    /// <summary>
    ///     Defines a provider of crypto keys
    /// </summary>
    public interface ICryptoKeyProvider
    {
        /// <summary>
        ///     Returns the <see cref="ICryptoKeyPair" /> for the specified key type.
        /// </summary>
        /// <param name="key">The type of the key to return</param>
        ICryptoKeyPair GetCryptoKey(CryptoKeyType key);
    }
}