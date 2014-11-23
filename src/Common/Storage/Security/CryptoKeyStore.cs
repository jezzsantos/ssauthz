using System.Collections.Generic;
using System.Linq;
using Common.Reflection;
using Common.Storage.DataEntities;
using DotNetOpenAuth.Messaging.Bindings;
using ServiceStack;

namespace Common.Storage.Security
{
    /// <summary>
    /// Provides a store for cryptographic keys
    /// </summary>
    public class CryptoKeyStore : ICryptoKeyStore
    {
        /// <summary>
        ///     Gets or sets the <see cref="IStorageProvider{ICryptoKey}" />
        /// </summary>
        public IStorageProvider<IOAuthCryptoKey> Storage { get; set; }

        /// <summary>
        ///     Gets the key from the bucket by the specified handle
        /// </summary>
        public CryptoKey GetKey(string bucket, string handle)
        {
            Guard.NotNullOrEmpty(() => bucket, bucket);
            Guard.NotNullOrEmpty(() => handle, handle);

            IOAuthCryptoKey key = GetKeyByBucketHandle(bucket, handle);
            if (key != null)
            {
                return new CryptoKey(key.Key, key.ExpiresUtc);
            }

            return null;
        }

        /// <summary>
        ///     Gets all keys from the specified bucket
        /// </summary>
        public IEnumerable<KeyValuePair<string, CryptoKey>> GetKeys(string bucket)
        {
            Guard.NotNullOrEmpty(() => bucket, bucket);

            return GetKeysByBucket(bucket)
                .OrderByDescending(key => key.ExpiresUtc)
                .Select(key => new KeyValuePair<string, CryptoKey>(key.Handle, new CryptoKey(key.Key, key.ExpiresUtc)));
        }

        /// <summary>
        ///     Removes the key from the specified bucket by the specified handle
        /// </summary>
        public void RemoveKey(string bucket, string handle)
        {
            Guard.NotNullOrEmpty(() => bucket, bucket);
            Guard.NotNullOrEmpty(() => handle, handle);

            IOAuthCryptoKey key = GetKeyByBucketHandle(bucket, handle);
            if (key != null)
            {
                Storage.Delete(key.Id);
            }
        }

        /// <summary>
        ///     Stores the specified key in the specified bucket with the specified handle
        /// </summary>
        public void StoreKey(string bucket, string handle, CryptoKey key)
        {
            Guard.NotNullOrEmpty(() => bucket, bucket);
            Guard.NotNullOrEmpty(() => handle, handle);
            Guard.NotNull(() => key, key);

            // Remove existing key
            RemoveKey(bucket, handle);

            Storage.Add(new OAuthCryptoKey
            {
                Key = key.Key,
                Bucket = bucket,
                Handle = handle,
                ExpiresUtc = key.ExpiresUtc,
            });
        }

        /// <summary>
        ///     Empties the store of all buckets
        /// </summary>
        /// <remarks>For testing only</remarks>
        internal void EmptyStore()
        {
            GetAllKeys()
                .ToList()
                .ForEach(k =>
                    Storage.Delete(k.Id));
        }

        private IOAuthCryptoKey GetKeyByBucketHandle(string bucket, string handle)
        {
            IEnumerable<IOAuthCryptoKey> keys = GetKeysByBucket(bucket);
            if (keys.Any())
            {
                return keys.FirstOrDefault(k => k.Handle.EqualsIgnoreCase(handle));
            }

            return null;
        }

        private IEnumerable<IOAuthCryptoKey> GetKeysByBucket(string bucket)
        {
            return Storage.Find(
                Storage.BuildQuery(Reflector<OAuthCryptoKey>.GetPropertyName(x => x.Bucket), QueryOperator.EQ,
                    bucket));
        }

        private IEnumerable<IOAuthCryptoKey> GetAllKeys()
        {
            return Storage.Find(
                Storage.BuildQuery(Reflector<OAuthCryptoKey>.GetPropertyName(x => x.Bucket), QueryOperator.NE,
                    string.Empty));
        }
    }
}