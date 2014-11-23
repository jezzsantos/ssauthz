using Common;
using Common.Services;
using Services.AuthZ.Properties;
using Services.DataContracts;
using ServiceStack.FluentValidation;

namespace Services.AuthZ.Validators
{
    /// <summary>
    ///     A validator for the <see cref="Address" /> contract.
    /// </summary>
    internal class AddressValidator : AbstractValidator<Address>
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="AddressValidator" /> class.
        /// </summary>
        public AddressValidator(string collectionName)
        {
            When(dto => dto.Street1.HasValue(), () =>
            {
                RuleFor(dto => dto.Street1).Matches(DataFormats.User.Address.Street.Expression)
                    .WithMessage(Resources.AddressValidator_InvalidAddressStreet);
            });
            When(dto => dto.Street2.HasValue(), () =>
            {
                RuleFor(dto => dto.Street2).Matches(DataFormats.User.Address.Street.Expression)
                    .WithMessage(Resources.AddressValidator_InvalidAddressStreet);
            });
            RuleFor(dto => dto.Town).Matches(DataFormats.User.Address.Town.Expression)
                .WithMessage(Resources.AddressValidator_InvalidAddressTown);
        }
    }
}