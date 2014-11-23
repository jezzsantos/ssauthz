using System;

namespace Common.Services
{
    public static class LogicErrorThrower
    {
        /// <summary>
        ///     Throws an unexpected exception
        /// </summary>
        /// <param name="inner">The inner exception</param>
        /// <param name="message">The message explaining the problem</param>
        /// <param name="args">Optional arguments to format the message</param>
        public static Exception Unexpected(Exception inner = null, string message = null, params object[] args)
        {
            string formattedMessage = (message.HasValue()
                ? message.FormatWith(args)
                : null);
            return (inner != null)
                ? new InvalidOperationException(inner.Message, inner)
                : new InvalidOperationException(formattedMessage);
        }

        /// <summary>
        ///     Throws a resource not found exception
        /// </summary>
        /// <param name="message">The message explaining the problem</param>
        /// <param name="args">Optional arguments to format the message</param>
        public static Exception ResourceNotFound(string message = null, params object[] args)
        {
            string formattedMessage = (message.HasValue()
                ? message.FormatWith(args)
                : null);
            return new ResourceNotFoundException(formattedMessage);
        }

        /// <summary>
        ///     Throws a resource conflict exception, usually when the resource already exists, and cannot be re-created.
        /// </summary>
        /// <param name="message">The message explaining the problem</param>
        /// <param name="args">Optional arguments to format the message</param>
        public static Exception ResourceConflict(string message = null, params object[] args)
        {
            string formattedMessage = (message.HasValue()
                ? message.FormatWith(args)
                : null);
            return new ResourceConflictException(formattedMessage);
        }

        /// <summary>
        ///     Throws a rule violation exception
        /// </summary>
        /// <param name="message">The message explaining the problem</param>
        /// <param name="args">Optional arguments to format the message</param>
        public static Exception RuleViolation(string message = null, params object[] args)
        {
            string formattedMessage = (message.HasValue()
                ? message.FormatWith(args)
                : null);
            return new RuleViolationException(formattedMessage);
        }
    }
}