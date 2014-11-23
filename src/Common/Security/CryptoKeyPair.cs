using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Common.Security
{
    /// <summary>
    ///     Defines a pair of asymetric keys, typically used for signing and encryption.
    /// </summary>
    internal class CryptoKeyPair : ICryptoKeyPair
    {
        private readonly RSACryptoServiceProvider publicKeyProvider;
        private readonly RSACryptoServiceProvider secretKeyProvider;

        /// <summary>
        ///     Creates a new instance of the <see cref="CryptoKeyPair" /> class.
        /// </summary>
        public CryptoKeyPair(byte[] certificateData, string certificatePassword)
            : this(new X509Certificate2(certificateData, certificatePassword))
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="CryptoKeyPair" /> class.
        /// </summary>
        public CryptoKeyPair(X509Certificate2 cert)
        {
            Guard.NotNull(() => cert, cert);

            publicKeyProvider = cert.PublicKey.Key as RSACryptoServiceProvider;
            secretKeyProvider = cert.PrivateKey as RSACryptoServiceProvider;
        }

        /// <summary>
        ///     Gets the public key (typically used for signing)
        /// </summary>
        public RSACryptoServiceProvider PublicSigningKey
        {
            get { return (publicKeyProvider); }
        }

        /// <summary>
        ///     Gets the private key (typically used for encryption)
        /// </summary>
        public RSACryptoServiceProvider PrivateEncryptionKey
        {
            get { return (secretKeyProvider); }
        }
    }
}