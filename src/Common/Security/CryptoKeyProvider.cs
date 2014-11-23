using System;
using System.Security.Cryptography.X509Certificates;
using Common.Configuration;
using Common.Properties;

namespace Common.Security
{
    /// <summary>
    ///     Provides a provider of crypto keys
    /// </summary>
    public class CryptoKeyProvider : ICryptoKeyProvider
    {
        private const StoreName CertStoreName = StoreName.My;
        private const StoreLocation CertStoreLocation = StoreLocation.LocalMachine;
        private const string CertConfigurationSettingFormat = @"{0}.{1}";
        private readonly string cryptoKeySettingName;

        /// <summary>
        ///     Creates a new instance of the <see cref="CryptoKeyProvider" /> class.
        /// </summary>
        public CryptoKeyProvider(string cryptoKeySettingName)
        {
            Guard.NotNullOrEmpty(() => cryptoKeySettingName, cryptoKeySettingName);

            this.cryptoKeySettingName = cryptoKeySettingName;
        }

        /// <summary>
        ///     Gets or sets the <see cref="IConfigurationSettings" />
        /// </summary>
        public IConfigurationSettings Configuration { get; set; }

        /// <summary>
        ///     Returns the <see cref="ICryptoKeyPair" /> for the specified key type.
        /// </summary>
        /// <param name="key">The type of the key to return</param>
        public ICryptoKeyPair GetCryptoKey(CryptoKeyType key)
        {
            Guard.NotNull(() => key, key);

            string settingName = CreateSettingName(key);
            string subjectName = GetSubjectName(settingName);

            return new CryptoKeyPair(LoadCertBySubjectName(subjectName));
        }

        private string GetSubjectName(string settingName)
        {
            string subjectName = Configuration.GetSetting(settingName);
            if (!subjectName.HasValue())
            {
                throw new InvalidOperationException(
                    Resources.CryptoKeyProvider_FailedToReadConfiguration.FormatWith(settingName));
            }

            return subjectName;
        }

        private string CreateSettingName(CryptoKeyType key)
        {
            return CertConfigurationSettingFormat.FormatWith(cryptoKeySettingName,
                key);
        }

        private static X509Certificate2 LoadCertBySubjectName(string subjectName)
        {
            // Find certs by subjectname in the store
            var store = new X509Store(CertStoreName, CertStoreLocation);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectName,
                subjectName, false);
            if (certs.Count == 0)
            {
                throw new InvalidOperationException(
                    Resources.CryptoKeyProvider_FailedToFindCertificate.FormatWith(subjectName, CertStoreName,
                        CertStoreLocation));
            }

            return certs[0];
        }
    }
}