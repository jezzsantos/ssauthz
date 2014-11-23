using System.Text.RegularExpressions;
using Common.Properties;
using ServiceStack.FluentValidation.Validators;

namespace Common.Services.Validators
{
    internal interface IUserIdValidator : IPropertyValidator
    {
    }

    /// <summary>
    ///     A validator for a user's username property value.
    /// </summary>
    internal class UsernameValidator : PropertyValidator, IUserIdValidator
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="UsernameValidator" /> class.
        /// </summary>
        public UsernameValidator()
            : base(() => Resources.UserIdValidator_ErrorMessage, "IsUsername")
        {
        }

        /// <summary>
        ///     Whether the property value of the context is valid
        /// </summary>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyValue == null)
            {
                return false;
            }

            string propertyValue = context.PropertyValue.ToString();

            return Regex.IsMatch(propertyValue, DataFormats.User.Username.Expression);
        }
    }
}