using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Common.Properties;
using ServiceStack.FluentValidation;
using ServiceStack.FluentValidation.Results;

namespace Common
{
    /// <summary>
    ///     Validates common arguments.
    /// </summary>
    [DebuggerStepThrough]
    public static class Guard
    {
        /// <summary>
        ///     Ensures the given <paramref name="value" /> is not null, otherwise throws <see cref="ArgumentNullException" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be validated.</typeparam>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        public static void NotNull<TValue>(Expression<Func<TValue>> reference, TValue value)
        {
            if (value == null)
            {
                NotNull<TValue, ArgumentNullException>(reference, value);
            }
        }

        /// <summary>
        ///     Ensures the given <paramref name="value" /> is not null, otherwise throws <see cref="ArgumentNullException" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be validated.</typeparam>
        /// <typeparam name="TException">The type of the exception to throw </typeparam>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        public static void NotNull<TValue, TException>(Expression<Func<TValue>> reference, TValue value)
        {
            if (value == null)
            {
                throw CreateException(typeof (TException), GetParameterName(reference),
                    Resources.Guard_ArgumentCanNotBeNull);
            }
        }

        /// <summary>
        ///     Ensures the given <paramref name="value" /> is not null, otherwise throws <see cref="ArgumentNullException" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be validated.</typeparam>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="messageOrFormat">Message to display if the value is null.</param>
        /// <param name="formatArgs">Optional arguments to format the <paramref name="messageOrFormat" />.</param>
        public static void NotNull<TValue>(Expression<Func<TValue>> reference, TValue value, string messageOrFormat,
            params object[] formatArgs)
        {
            if (value == null)
            {
                if (messageOrFormat.HasValue())
                {
                    if (formatArgs != null && formatArgs.Length != 0)
                    {
                        throw new ArgumentNullException(GetParameterName(reference),
                            messageOrFormat.FormatWith(formatArgs));
                    }
                    throw new ArgumentNullException(GetParameterName(reference), messageOrFormat);
                }
                throw new ArgumentNullException(GetParameterName(reference), Resources.Guard_ArgumentCanNotBeNull);
            }
        }

        /// <summary>
        ///     Ensures the given <paramref name="value" /> is not null, otherwise throws <see cref="ArgumentNullException" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be validated.</typeparam>
        /// <typeparam name="TException">The type of the exception to throw </typeparam>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="messageOrFormat">Message to display if the value is null.</param>
        /// <param name="formatArgs">Optional arguments to format the <paramref name="messageOrFormat" />.</param>
        public static void NotNull<TValue, TException>(Expression<Func<TValue>> reference, TValue value,
            string messageOrFormat,
            params object[] formatArgs)
        {
            if (value == null)
            {
                if (messageOrFormat.HasValue())
                {
                    if (formatArgs != null && formatArgs.Length != 0)
                    {
                        throw CreateException(typeof (TException), GetParameterName(reference),
                            messageOrFormat.FormatWith(formatArgs));
                    }
                    throw CreateException(typeof (TException), GetParameterName(reference), messageOrFormat);
                }
                throw CreateException(typeof (TException), GetParameterName(reference),
                    Resources.Guard_ArgumentCanNotBeNull);
            }
        }

        /// <summary>
        ///     Ensures the given string <paramref name="value" /> is not null or empty. Throws <see cref="TException" />
        ///     in both cases.
        /// </summary>
        /// <typeparam name="TException">The type of the exception to throw </typeparam>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        public static void NotNullOrEmpty<TException>(Expression<Func<string>> reference, string value)
        {
            NotNull<string, TException>(reference, value);
            if (value.Length == 0)
            {
                throw CreateException(typeof (TException), GetParameterName(reference),
                    Resources.Guard_ArgumentCanNotBeEmpty);
            }
        }

        /// <summary>
        ///     Ensures the given string <paramref name="value" /> is not null or empty. Throws
        ///     <see cref="ArgumentNullException" />
        ///     in the first case, or <see cref="ArgumentException" /> in the latter.
        /// </summary>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        public static void NotNullOrEmpty(Expression<Func<string>> reference, string value)
        {
            NotNull(reference, value);
            if (value.Length == 0)
            {
                NotNullOrEmpty<ArgumentOutOfRangeException>(reference, value);
            }
        }

        /// <summary>
        ///     Ensures the given string <paramref name="value" /> is not null or empty. Throws
        ///     <see cref="ArgumentNullException" />
        ///     in the first case, or <see cref="ArgumentException" /> in the latter.
        /// </summary>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="messageOrFormat">Message to display if the value is null.</param>
        /// <param name="formatArgs">Optional arguments to format the <paramref name="messageOrFormat" />.</param>
        public static void NotNullOrEmpty(Expression<Func<string>> reference, string value, string messageOrFormat,
            params object[] formatArgs)
        {
            NotNull(reference, value, messageOrFormat, formatArgs);
            if (value.Length == 0)
            {
                if (messageOrFormat.HasValue())
                {
                    if (formatArgs != null && formatArgs.Length != 0)
                    {
                        throw new ArgumentOutOfRangeException(GetParameterName(reference),
                            messageOrFormat.FormatWith(formatArgs));
                    }
                    throw new ArgumentOutOfRangeException(GetParameterName(reference), messageOrFormat);
                }
                throw new ArgumentOutOfRangeException(GetParameterName(reference),
                    Resources.Guard_ArgumentCanNotBeEmpty);
            }
        }

        /// <summary>
        ///     Ensures the given string <paramref name="value" /> is not null or empty. Throws
        ///     <see cref="ArgumentNullException" />
        ///     in the first case, or <see cref="ArgumentException" /> in the latter.
        /// </summary>
        /// <typeparam name="TException">The type of the exception to throw </typeparam>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        /// <param name="messageOrFormat">Message to display if the value is null.</param>
        /// <param name="formatArgs">Optional arguments to format the <paramref name="messageOrFormat" />.</param>
        public static void NotNullOrEmpty<TException>(Expression<Func<string>> reference, string value,
            string messageOrFormat,
            params object[] formatArgs)
        {
            NotNull(reference, value, messageOrFormat, formatArgs);
            if (value.Length == 0)
            {
                if (messageOrFormat.HasValue())
                {
                    if (formatArgs != null && formatArgs.Length != 0)
                    {
                        throw CreateException(typeof (TException), GetParameterName(reference),
                            messageOrFormat.FormatWith(formatArgs));
                    }
                    throw CreateException(typeof (TException), GetParameterName(reference), messageOrFormat);
                }
                throw CreateException(typeof (TException), GetParameterName(reference),
                    Resources.Guard_ArgumentCanNotBeEmpty);
            }
        }

        /// <summary>
        ///     Ensures that the value is of the given expected type.
        /// </summary>
        public static void OfType<T>(Expression<Func<T>> reference, T value, Type expectedType)
        {
            if (value != null && !expectedType.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentNullException(GetParameterName(reference),
                    Resources.Guard_ArgumentTypeNotExpected.FormatWith(expectedType.FullName));
            }
        }

        /// <summary>
        ///     Validates the given string is valid with respect to the specified validator
        /// </summary>
        /// <typeparam name="TValidator"></typeparam>
        /// <typeparam name="TObject"> </typeparam>
        /// <param name="validator">The validator to use.</param>
        /// <param name="reference">The expression used to extract the name of the parameter.</param>
        /// <param name="value">The value to check.</param>
        public static void Validate<TValidator, TObject>(TValidator validator, Expression<Func<TObject>> reference,
            TObject value)
            where TValidator : IValidator<TObject>
            where TObject : class
        {
            ValidationResult error = validator.Validate(value);
            if (!error.IsValid)
            {
                throw new ArgumentOutOfRangeException(GetParameterName(reference),
                    Resources.Guard_Validate_FailedValidation.FormatWith(string.Join(",",
                        error.Errors.Select(err => err.ErrorMessage))));
            }
        }

        private static string GetParameterName<T>(Expression<T> reference)
        {
            return ((MemberExpression) reference.Body).Member.Name;
        }

        private static Exception CreateException(Type exceptionType, string parameterName, string message)
        {
            if (exceptionType.IsAssignableFrom(typeof (ArgumentException)))
            {
                return Activator.CreateInstance(exceptionType, new object[]
                {
                    parameterName,
                    message
                }) as ArgumentException;
            }

            return Activator.CreateInstance(exceptionType, new object[]
            {
                message
            }) as Exception;
        }
    }
}