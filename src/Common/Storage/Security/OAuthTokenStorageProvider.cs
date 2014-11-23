using System;

namespace Common.Storage.Security
{
    /// <summary>
    ///     A storage provider for <see cref="OAuthToken" />
    /// </summary>
    public class OAuthTokenStorageProvider : BaseStorageProvider<IOAuthToken, OAuthTokenEntity>
    {
        /// <summary>
        ///     Converts the <see cref="OAuthTokenEntity" /> to a DTO of <see cref="IOAuthToken" />
        /// </summary>
        protected override IOAuthToken ToDto(OAuthTokenEntity entity)
        {
            return entity.ToDto();
        }

        /// <summary>
        ///     Converts the DTO <see cref="OAuthToken" /> to a entity of <see cref="OAuthTokenEntity" />
        /// </summary>
        protected override OAuthTokenEntity FromDto(IOAuthToken contract)
        {
            return OAuthTokenEntity.FromDto(contract);
        }

        /// <summary>
        ///     Creates a new identity for the new entity
        /// </summary>
        protected override string CreateIdentity(OAuthTokenEntity entity)
        {
            return Guid.NewGuid().ToString();
        }
    }
}