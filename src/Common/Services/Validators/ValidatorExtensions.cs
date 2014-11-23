using ServiceStack.FluentValidation;

namespace Common.Services.Validators
{
    /// <summary>
    ///     Extensions for validators
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        ///     Defines a validator that will fail if the property is not a valid ID of an entity.
        /// </summary>
        public static IRuleBuilderOptions<T, TProperty> IsEntityId<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new EntityIdValidator());
        }

        /// <summary>
        ///     Defines a validator that will fail if the property is not a valid identifier for a user
        /// </summary>
        public static IRuleBuilderOptions<T, TProperty> IsUsername<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UsernameValidator());
        }

        /// <summary>
        ///     Defines a validator that will fail if the property is not a valid hash for a password
        /// </summary>
        public static IRuleBuilderOptions<T, TProperty> IsPasswordHash<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new PasswordHashValidator());
        }
    }
}