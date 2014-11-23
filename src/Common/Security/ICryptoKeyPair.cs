using System.Security.Cryptography;

namespace Common.Security
{
    /// <summary>
    ///     Represents a RSA crypto key pair
    /// </summary>
    public interface ICryptoKeyPair
    {
        /// <summary>
        ///     The public key used to encrypt access tokens and verify signatures.
        /// </summary>
        RSACryptoServiceProvider PublicSigningKey { get; }

        /// <summary>
        ///     The private to sign and decrypt access tokens.
        /// </summary>
        RSACryptoServiceProvider PrivateEncryptionKey { get; }
    }
}