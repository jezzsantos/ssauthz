using System;
using System.Globalization;
using Common.Text;

namespace Common.Storage.DataEntities
{
    /// <summary>
    /// Helpers for storage entities
    /// </summary>
    public static class EntityHelper
    {
        /// <summary>
        ///     Creates a unique row key for the entry.
        /// </summary>
        /// <returns></returns>
        public static string CreateRowKey()
        {
            return String.Format(CultureInfo.InvariantCulture,
                @"{0:10}_{1}", (DateTime.MaxValue.Ticks - DateTime.Now.Ticks), Guid.NewGuid());
        }

        /// <summary>
        ///     Serializes the object for storage
        /// </summary>
        /// <param name="instance">The object to serialize</param>
        /// <returns></returns>
        public static string SerializeForStorage(object instance)
        {
            return Serialization.Serialize(instance);
        }

        /// <summary>
        ///     Deserializes the string representation from storage into an instance
        /// </summary>
        /// <typeparam name="T">The type of object being deserialized</typeparam>
        /// <param name="serializedData">The data to be deserialized</param>
        /// <param name="defaultValue">A default value should the data be undefined</param>
        /// <returns></returns>
        public static T DeserializeFromStorage<T>(string serializedData, T defaultValue)
        {
            return Serialization.Deserialize(serializedData, defaultValue);
        }
    }
}