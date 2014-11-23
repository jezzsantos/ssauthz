using Common;
using Common.Reflection;
using Common.Services;
using Common.Services.Validators;
using Services.AuthZ.Properties;
using Services.MessageContracts;
using ServiceStack.FluentValidation;

namespace Services.AuthZ.Validators
{
    partial class UpdateUserAccountValidator
    {
        partial void AddRules()
        {
            When(dto => dto.Id.HasValue(), () =>
            {
                RuleFor(dto => dto.Id).IsEntityId()
                    .WithMessage(Resources.UpdateUserAccountValidator_InvalidId);
            });
            When(dto => dto.Forenames.HasValue(), () =>
            {
                RuleFor(dto => dto.Forenames).Matches(DataFormats.User.Forenames.Expression)
                    .WithMessage(Resources.UpdateUserAccountValidator_InvalidForenames);
            });
            When(dto => dto.Surname.HasValue(), () =>
            {
                RuleFor(dto => dto.Surname).Matches(DataFormats.User.Surname.Expression)
                    .WithMessage(Resources.UpdateUserAccountValidator_InvalidSurname);
            });
            When(dto => dto.Address != null, () =>
            {
                RuleFor(dto => dto.Address)
                    .SetValidator(new AddressValidator(Reflector<UpdateUserAccount>.GetPropertyName(x => x.Address)));
            });
            When(dto => dto.Email.HasValue(), () =>
            {
                RuleFor(dto => dto.Email).EmailAddress()
                    .WithMessage(Resources.UpdateUserAccountValidator_InvalidEmail);
            });
            When(dto => dto.MobilePhone.HasValue(), () =>
            {
                RuleFor(dto => dto.MobilePhone).Matches(DataFormats.User.Phone.Expression)
                    .WithMessage(Resources.UpdateUserAccountValidator_InvalidMobilePhone);
            });
            When(dto => dto.NewPasswordHash.HasValue(), () =>
            {
                RuleFor(dto => dto.OldPasswordHash).NotNull();
                When(dto => dto.OldPasswordHash.HasValue(), () =>
                {
                    RuleFor(dto => dto.OldPasswordHash).IsPasswordHash()
                        .WithMessage(Resources.UpdateUserAccountValidator_InvalidOldPasswordHash);
                });
                RuleFor(dto => dto.NewPasswordHash).IsPasswordHash()
                    .WithMessage(Resources.UpdateUserAccountValidator_InvalidNewPasswordHash);
            });
        }
    }
}