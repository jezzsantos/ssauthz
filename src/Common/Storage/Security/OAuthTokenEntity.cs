using System;
using Common.Storage.DataEntities;

namespace Common.Storage.Security
{
    /// <summary>
    ///     An azure storage table definition for an <see cref="IOAuthToken" />
    /// </summary>
    public class OAuthTokenEntity : KeyedEntity
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="OAuthTokenEntity" /> class.
        /// </summary>
        public OAuthTokenEntity()
        {
            RowKey = EntityHelper.CreateRowKey();
        }

        /// <summary>
        ///     Gets the username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     Gets the access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        ///     Gets the refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        ///     Gets the date the token expires
        /// </summary>
        public string ExpiresOnUtc { get; set; }

        /// <summary>
        ///     Gets the date the token was issued
        /// </summary>
        public string IssuedOnUtc { get; set; }

        /// <summary>
        ///     Gets an entity from the <see cref="IOAuthToken" />.
        /// </summary>
        public static OAuthTokenEntity FromDto(IOAuthToken dto)
        {
            Guard.NotNull(() => dto, dto);

            return new OAuthTokenEntity
            {
                Id = EntityHelper.SerializeForStorage(dto.Id),
                Username = EntityHelper.SerializeForStorage(dto.Username),
                AccessToken = EntityHelper.SerializeForStorage(dto.AccessToken),
                RefreshToken = EntityHelper.SerializeForStorage(dto.RefreshToken),
                ExpiresOnUtc = EntityHelper.SerializeForStorage(dto.ExpiresOnUtc),
                IssuedOnUtc = EntityHelper.SerializeForStorage(dto.IssuedOnUtc),
            };
        }

        /// <summary>
        ///     Gets the <see cref="IOAuthToken" /> from the entity.
        /// </summary>
        public virtual IOAuthToken ToDto()
        {
            var dto = new OAuthToken
            {
                Id = EntityHelper.DeserializeFromStorage(Id, string.Empty),
                Username = EntityHelper.DeserializeFromStorage(Username, String.Empty),
                AccessToken = EntityHelper.DeserializeFromStorage(AccessToken, String.Empty),
                RefreshToken = EntityHelper.DeserializeFromStorage(RefreshToken, String.Empty),
                ExpiresOnUtc = EntityHelper.DeserializeFromStorage(ExpiresOnUtc, DateTime.MinValue),
                IssuedOnUtc = EntityHelper.DeserializeFromStorage(IssuedOnUtc, DateTime.MinValue),
            };

            return dto;
        }

        /// <summary>
        ///     Merges the properties from the <see cref="entity" /> into this entity
        /// </summary>
        /// <param name="entity">The entity to merge</param>
        public override void Merge(KeyedEntity entity)
        {
            Guard.NotNull(() => entity, entity);

            var mergeEntity = entity as OAuthTokenEntity;
            if (mergeEntity == null)
            {
                return;
            }

            IOAuthToken thisDto = ToDto();
            IOAuthToken mergeDto = mergeEntity.ToDto();

            // Compare to see if we need to update changed values (null) values are ignored in persistence
            thisDto.Username = mergeDto.Username.HasValue() ? mergeDto.Username : thisDto.Username;
            thisDto.AccessToken = mergeDto.AccessToken.HasValue()
                ? mergeDto.AccessToken
                : thisDto.AccessToken;
            thisDto.RefreshToken = mergeDto.RefreshToken.HasValue()
                ? mergeDto.RefreshToken
                : thisDto.RefreshToken;
            thisDto.ExpiresOnUtc = mergeDto.ExpiresOnUtc.HasValue() ? mergeDto.ExpiresOnUtc : thisDto.ExpiresOnUtc;
            thisDto.IssuedOnUtc = mergeDto.IssuedOnUtc.HasValue() ? mergeDto.IssuedOnUtc : thisDto.IssuedOnUtc;

            //Convert back to entity
            OAuthTokenEntity thisEntity = FromDto(thisDto);
            Username = thisEntity.Username;
            AccessToken = thisEntity.AccessToken;
            RefreshToken = thisEntity.RefreshToken;
            ExpiresOnUtc = thisEntity.ExpiresOnUtc;
            IssuedOnUtc = thisEntity.IssuedOnUtc;
        }
    }
}