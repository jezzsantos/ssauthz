using System;

namespace Common.Storage.Security
{
    /// <summary>
    ///     A storage provider for <see cref="OAuthCryptoKey" />
    /// </summary>
    public class OAuthCryptoKeyStorageProvider : BaseStorageProvider<IOAuthCryptoKey, OAuthCryptoKeyEntity>
    {
        /// <summary>
        ///     Converts the <see cref="OAuthCryptoKeyEntity" /> to a DTO of <see cref="IOAuthCryptoKey" />
        /// </summary>
        protected override IOAuthCryptoKey ToDto(OAuthCryptoKeyEntity entity)
        {
            return entity.ToDto();
        }

        /// <summary>
        ///     Converts the DTO <see cref="OAuthCryptoKey" /> to a entity of <see cref="OAuthCryptoKeyEntity" />
        /// </summary>
        protected override OAuthCryptoKeyEntity FromDto(IOAuthCryptoKey contract)
        {
            return OAuthCryptoKeyEntity.FromDto(contract);
        }

        /// <summary>
        ///     Creates a new identity for the new entity
        /// </summary>
        protected override string CreateIdentity(OAuthCryptoKeyEntity entity)
        {
            return Guid.NewGuid().ToString();
        }
    }
}