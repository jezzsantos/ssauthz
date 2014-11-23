using System;
using Common.Storage.DataEntities;

namespace Common.Storage.Security
{
    /// <summary>
    ///     An azure storage table definition for an <see cref="IOAuthCryptoKey" />
    /// </summary>
    public class OAuthCryptoKeyEntity : KeyedEntity
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="OAuthCryptoKeyEntity" /> class.
        /// </summary>
        public OAuthCryptoKeyEntity()
        {
            RowKey = EntityHelper.CreateRowKey();
        }

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
        public string Key { get; set; }

        /// <summary>
        ///     Gets the expiry date
        /// </summary>
        public string ExpiresUtc { get; set; }

        /// <summary>
        ///     Gets an entity from the <see cref="IOAuthCryptoKey" />.
        /// </summary>
        public static OAuthCryptoKeyEntity FromDto(IOAuthCryptoKey dto)
        {
            Guard.NotNull(() => dto, dto);

            return new OAuthCryptoKeyEntity
            {
                Id = EntityHelper.SerializeForStorage(dto.Id),
                Bucket = EntityHelper.SerializeForStorage(dto.Bucket),
                Handle = EntityHelper.SerializeForStorage(dto.Handle),
                Key = EntityHelper.SerializeForStorage(dto.Key),
                ExpiresUtc = EntityHelper.SerializeForStorage(dto.ExpiresUtc),
            };
        }

        /// <summary>
        ///     Gets the <see cref="IOAuthCryptoKey" /> from the entity.
        /// </summary>
        public virtual IOAuthCryptoKey ToDto()
        {
            var dto = new OAuthCryptoKey
            {
                Id = EntityHelper.DeserializeFromStorage(Id, string.Empty),
                Bucket = EntityHelper.DeserializeFromStorage(Bucket, String.Empty),
                Handle = EntityHelper.DeserializeFromStorage(Handle, String.Empty),
                Key = EntityHelper.DeserializeFromStorage<byte[]>(Key, null),
                ExpiresUtc = EntityHelper.DeserializeFromStorage(ExpiresUtc, DateTime.MinValue),
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

            var mergeEntity = entity as OAuthCryptoKeyEntity;
            if (mergeEntity == null)
            {
                return;
            }

            IOAuthCryptoKey thisDto = ToDto();
            IOAuthCryptoKey mergeDto = mergeEntity.ToDto();

            // Compare to see if we need to update changed values (null) values are ignored in persistence
            thisDto.Bucket = mergeDto.Bucket.HasValue() ? mergeDto.Bucket : thisDto.Bucket;
            thisDto.Handle = mergeDto.Handle.HasValue() ? mergeDto.Handle : thisDto.Handle;
            thisDto.Key = mergeDto.Key ?? thisDto.Key;
            thisDto.ExpiresUtc = mergeDto.ExpiresUtc.HasValue() ? mergeDto.ExpiresUtc : thisDto.ExpiresUtc;

            //Convert back to entity
            OAuthCryptoKeyEntity thisEntity = FromDto(thisDto);
            Bucket = thisEntity.Bucket;
            Handle = thisEntity.Handle;
            Key = thisEntity.Key;
            ExpiresUtc = thisEntity.ExpiresUtc;
        }
    }
}