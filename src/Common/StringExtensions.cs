using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using ServiceStack;

namespace Common
{
    /// <summary>
    ///     Provides formatting of strings using object properties.
    /// </summary>
    [DebuggerStepThrough]
    public static class StringExtensions
    {
        /// <summary>
        ///     Whether the specified value is string representation of a <see cref="DateTime" />
        /// </summary>
        public static bool IsDateTime(this string value)
        {
            DateTime dateValue;
            return DateTime.TryParse(value, out dateValue);
        }

        /// <summary>
        ///     Whether the specified value is a string representation of a <see cref="Guid" />.
        /// </summary>
        public static bool IsGuid(this string value)
        {
            Guid guid;
            return Guid.TryParse(value, out guid);
        }

        /// <summary>
        ///     Whether the specified value in not null and not empty
        /// </summary>
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        ///     Whether the specified value is exactly equal to other value
        /// </summary>
        public static bool EqualsOrdinal(this string value, string other)
        {
            return String.Equals(value, other, StringComparison.Ordinal);
        }

        /// <summary>
        ///     Whether the specified value is not exactly equal to other value
        /// </summary>
        public static bool NotEqualsOrdinal(this string value, string other)
        {
            return !value.EqualsOrdinal(other);
        }

        /// <summary>
        ///     Whether the specified value is not equal to other value
        /// </summary>
        public static bool NotEqualsIgnoreCase(this string value, string other)
        {
            return !value.EqualsIgnoreCase(other);
        }

        /// <summary>
        ///     Formats the current string with the <see cref="CultureInfo.CurrentCulture" />
        /// </summary>
        /// <param name="format">The format of the string</param>
        /// <param name="args">Arguments to substitute into string</param>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        /// <summary>
        ///     Formats the current string with the <see cref="CultureInfo.InvariantCulture" />
        /// </summary>
        /// <param name="format">The format of the string</param>
        /// <param name="args">Arguments to substitute into string</param>
        public static string FormatWithInvariant(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        ///     Makes a string camel cased.
        /// </summary>
        /// <param name="identifier">The identifier to camel case</param>
        public static string MakeCamel(this string identifier)
        {
            if (identifier.Length <= 2)
            {
                return identifier.ToLower(CultureInfo.InvariantCulture);
            }
            if (char.IsUpper(identifier[0]))
            {
                return
                    char.ToLower(identifier[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) +
                    identifier.Substring(1);
            }
            return identifier;
        }

        /// <summary>
        ///     Whether the <see cref="formattedString" /> has been formatted from the specified <see cref="formatString" />.
        /// </summary>
        /// <remarks>
        ///     This function is useful for comparing two strings where the <see cref="formattedString" /> is the result of a
        ///     String.Format operation on
        ///     the <see cref="formatString" />, with one or more format substitutions.
        ///     For example: Calling this function with a string "My code is 5" and a resource string "My code is {0}" that
        ///     contains one or more formatting arguments, return <c>true</c>
        /// </remarks>
        public static bool IsFormattedFrom(this string formattedString, string formatString)
        {
            string escapedPattern = formatString
                .Replace("[", "\\[")
                .Replace("]", "\\]")
                .Replace("(", "\\(")
                .Replace(")", "\\)")
                .Replace(".", "\\.")
                .Replace("<", "\\<")
                .Replace(">", "\\>");

            string pattern = Regex.Replace(escapedPattern, @"\{\d+\}", ".*")
                .Replace(" ", @"\s");

            return new Regex(pattern).IsMatch(formattedString);
        }

        /// <summary>
        ///     Removes the last trailing slash (<see cref="Path.AltDirectorySeparatorChar" />) from path.
        /// </summary>
        public static string WithoutTrailingSlash(this string path)
        {
            return path.Trim(Path.AltDirectorySeparatorChar);
        }
    }
}