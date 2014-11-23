using System;
using System.Runtime.Serialization;

namespace Common.Services
{
    /// <summary>
    ///     Defines the <see cref="RuleViolationException" /> exception.
    /// </summary>
    [Serializable]
    public class RuleViolationException : Exception
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="RuleViolationException" /> class.
        /// </summary>
        public RuleViolationException()
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="RuleViolationException" /> class.
        /// </summary>
        /// <paramref name="message">The message of the exception</paramref>
        public RuleViolationException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="RuleViolationException" /> class.
        /// </summary>
        /// <paramref name="message">The message of the exception</paramref>
        /// <paramref name="inner">The inner exception</paramref>
        public RuleViolationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="RuleViolationException" /> class.
        /// </summary>
        /// <paramref name="info">The serialization information</paramref>
        /// <paramref name="context">The context for the exception</paramref>
        protected RuleViolationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}