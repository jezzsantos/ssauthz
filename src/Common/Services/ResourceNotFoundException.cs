using System;
using System.Runtime.Serialization;

namespace Common.Services
{
    /// <summary>
    ///     Defines the <see cref="ResourceNotFoundException" /> exception.
    /// </summary>
    [Serializable]
    public class ResourceNotFoundException : Exception
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="ResourceNotFoundException" /> class.
        /// </summary>
        public ResourceNotFoundException()
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ResourceNotFoundException" /> class.
        /// </summary>
        /// <paramref name="message">The message of the exception</paramref>
        public ResourceNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ResourceNotFoundException" /> class.
        /// </summary>
        /// <paramref name="message">The message of the exception</paramref>
        /// <paramref name="inner">The inner exception</paramref>
        public ResourceNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ResourceNotFoundException" /> class.
        /// </summary>
        /// <paramref name="info">The serialization information</paramref>
        /// <paramref name="context">The context for the exception</paramref>
        protected ResourceNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}