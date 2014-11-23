using System.Text.RegularExpressions;
using Common.Properties;
using ServiceStack.FluentValidation.Validators;

namespace Common.Services.Validators
{
    public interface IEntityIdValidator : IPropertyValidator
    {
    }

    /// <summary>
    ///     A validator for a entity's identifier property value.
    /// </summary>
    internal class EntityIdValidator : PropertyValidator, IEntityIdValidator
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="EntityIdValidator" /> class.
        /// </summary>
        public EntityIdValidator()
            : base(() => Resources.EntityIdValidator_ErrorMessage, "IsEntityId")
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

            return Regex.IsMatch(propertyValue, DataFormats.EntityIdentifier.Expression);
        }
    }
}