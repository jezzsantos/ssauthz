using System.Text.RegularExpressions;
using Common.Properties;
using ServiceStack.FluentValidation.Validators;

namespace Common.Services.Validators
{
    internal interface IPasswordHashValidator : IPropertyValidator
    {
    }

    /// <summary>
    ///     A validator for a password hash
    /// </summary>
    internal class PasswordHashValidator : PropertyValidator, IPasswordHashValidator
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="PasswordHashValidator" /> class.
        /// </summary>
        public PasswordHashValidator()
            : base(() => Resources.PasswordHashValidator_ErrorMessage, "IsPasswordHash")
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

            return Regex.IsMatch(propertyValue, DataFormats.User.PasswordHash.Expression);
        }
    }
}