using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Common.Services
{
    /// <summary>
    ///     Defines the data formats for the soution
    /// </summary>
    public static class DataFormats
    {
        public static readonly DataFormat EntityIdentifier = new DataFormat(@"^[0-9a-gA-G/-]{36}$", 36,
            36);

        /// <summary>
        ///     Whether the specified value is a valid username
        /// </summary>
        public static bool IsUsername(string value)
        {
            return Regex.IsMatch(value, User.Username.Expression);
        }

        /// <summary>
        ///     Whether the specified value is a valid entity identifier
        /// </summary>
        public static bool IsEntityId(string value)
        {
            return Regex.IsMatch(value, EntityIdentifier.Expression);
        }

        /// <summary>
        ///     Data formats for emails
        /// </summary>
        public static class Email
        {
            public static readonly DataFormat TemplateId = new DataFormat(@"^([\d\w]{5,30})|([0-9a-gA-G/-]{36})$", 5, 36);
        }

        /// <summary>
        ///     Data fromats for users
        /// </summary>
        public static class User
        {
            public static readonly DataFormat Username = new DataFormat(@"^[\w\d\.]{5,25}$", 5, 25);
            public static readonly DataFormat Password = new DataFormat(@"^[\w]{6,100}$", 6, 100);

            public static readonly DataFormat PasswordHash =
                new DataFormat(@"^sha1\:1000\:[\w\d\/\+]{32}\:[\w\d\/\+]{32}$", 32, 32);

            public static readonly DataFormat Forenames = new DataFormat(@"^[\w]{2,25}$", 2, 25);
            public static readonly DataFormat Surname = new DataFormat(@"^[\w]{2,25}$", 2, 25);

            public static readonly DataFormat Phone =
                new DataFormat(@"^(\+[1-9][\d]*(\([\d]*\)|-[\d]*-))?[0]?[1-9][\d\- ]*$", 5, 25);

            public static class Address
            {
                public static readonly DataFormat Street = new DataFormat(@"^[\d\w \-\,]{4,30}$", 4, 30);
                public static readonly DataFormat Town = new DataFormat(@"^[\w \-\,]{3,30}$", 3, 30);
            }
        }
    }

    public class DataFormat
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="DataFormat" /> class.
        /// </summary>
        public DataFormat(string expression, int minLength = 0, int maxLength = 0,
            IEnumerable<string> substitutions = null)
        {
            Expression = expression;
            MinLength = minLength;
            MaxLength = maxLength;
            Substitutions = substitutions ?? new List<string>();
        }

        /// <summary>
        ///     Gets the regular expression
        /// </summary>
        public string Expression { get; private set; }

        /// <summary>
        ///     Gets the maximum string length
        /// </summary>
        public int MaxLength { get; private set; }

        /// <summary>
        ///     Gets the minimum string length
        /// </summary>
        public int MinLength { get; private set; }

        /// <summary>
        ///     Gets the substitutions in the Expression
        /// </summary>
        public IEnumerable<string> Substitutions { get; private set; }

        /// <summary>
        ///     Substitutes the given name/values into the expression.
        /// </summary>
        public string Substitute(IDictionary<string, string> values)
        {
            Guard.NotNull(() => values, values);

            string expression = Expression;
            values.ToList()
                .ForEach(val =>
                {
                    if (Substitutions.Contains(val.Key))
                    {
                        expression = expression.Replace(val.Key, val.Value);
                    }
                });

            return expression;
        }

        /// <summary>
        ///     Substitutes the given values into the expression.
        /// </summary>
        /// <remarks>
        ///     Substitutions are performed by index
        /// </remarks>
        public string Substitute(IEnumerable<string> values)
        {
            return Substitute(InitializeSubstitutions(values));
        }

        private IDictionary<string, string> InitializeSubstitutions(IEnumerable<string> values)
        {
            Guard.NotNull(() => values, values);

            var result = new Dictionary<string, string>();

            List<string> substitutions = Substitutions.ToList();
            int counter = 0;
            values.ToList().ForEach(val =>
            {
                if (substitutions.Count > counter)
                {
                    result.Add(substitutions[counter], val);

                    counter++;
                }
            });

            return result;
        }
    }
}