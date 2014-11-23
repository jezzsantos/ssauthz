using System;
using ServiceStack.Text;

namespace Common.Text
{
    /// <summary>
    ///     Serialization for all persistence
    /// </summary>
    public static class Serialization
    {
        /// <remarks>
        ///     Seems to be the only DateHandler that actually works when we serialize/deserialize DateTime from storage
        /// </remarks>
        private const DateHandler CurrentDateHandler = DateHandler.ISO8601;

        /// <summary>
        ///     Serializes the object for persistence
        /// </summary>
        /// <param name="instance">The object to serialize</param>
        /// <param name="format">The format of serialization to perform</param>
        /// <returns></returns>
        public static string Serialize(object instance, SerializationFormat format = SerializationFormat.Json)
        {
            if (format != SerializationFormat.Json)
            {
                throw new NotImplementedException();
            }

            if (instance is string)
            {
                return instance as string;
            }

            string result = string.Empty;
            WithinJsonScope(() => { result = JsonSerializer.SerializeToString(instance); });

            return result;
        }

        /// <summary>
        ///     Returns true if the specified instance is of the specified type
        /// </summary>
        /// <typeparam name="T">The type of object being deserialized</typeparam>
        /// <param name="serializedData">The data to be deserialized</param>
        /// <param name="format">The format of serialization to perform</param>
        public static bool IsDeserializableAs<T>(string serializedData,
            SerializationFormat format = SerializationFormat.Json)
        {
            if (format != SerializationFormat.Json)
            {
                throw new NotImplementedException();
            }

            if (serializedData == null)
            {
                return false;
            }

            try
            {
                // TODO: replace this with a more reliable method
                JsonSerializer.DeserializeFromString<T>(serializedData);
                return true;
            }
            catch (FormatException)
            {
            }

            return false;
        }

        /// <summary>
        ///     Deserializes the string representation from persistence into an instance
        /// </summary>
        /// <typeparam name="T">The type of object being deserialized</typeparam>
        /// <param name="serializedData">The data to be deserialized</param>
        /// <param name="defaultValue">A default value should the data be undefined</param>
        /// <param name="format">The format of serialization to perform</param>
        /// <returns></returns>
        public static T Deserialize<T>(string serializedData, T defaultValue,
            SerializationFormat format = SerializationFormat.Json)
        {
            if (format != SerializationFormat.Json)
            {
                throw new NotImplementedException();
            }

            T result = default(T);
            WithinJsonScope(() =>
            {
                result = (serializedData.HasValue())
                    ? JsonSerializer.DeserializeFromString<T>(serializedData)
                    : defaultValue;
            });

            return result;
        }

        private static void WithinJsonScope(Action action)
        {
            using (JsConfigScope config = JsConfig.BeginScope())
            {
                SetJsonConfig(config);

                action();
            }
        }

        private static void SetJsonConfig(JsConfigScope scope)
        {
            // Assume all dates are UTC dates
            scope.DateHandler = CurrentDateHandler;
            scope.AssumeUtc = true;
            scope.AlwaysUseUtc = true;
        }
    }

    public enum SerializationFormat
    {
        Json = 0,
        Other = 1,
    }
}