using System;
using System.Runtime.Serialization;

namespace Common.Services
{
    /// <summary>
    ///     Defines the <see cref="ResourceConflictException" /> exception.
    /// </summary>
    [Serializable]
    public class ResourceConflictException : Exception
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="ResourceConflictException" /> class.
        /// </summary>
        public ResourceConflictException()
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ResourceConflictException" /> class.
        /// </summary>
        /// <paramref name="message">The message of the exception</paramref>
        public ResourceConflictException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ResourceConflictException" /> class.
        /// </summary>
        /// <paramref name="message">The message of the exception</paramref>
        /// <paramref name="inner">The inner exception</paramref>
        public ResourceConflictException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ResourceConflictException" /> class.
        /// </summary>
        /// <paramref name="info">The serialization information</paramref>
        /// <paramref name="context">The context for the exception</paramref>
        protected ResourceConflictException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}