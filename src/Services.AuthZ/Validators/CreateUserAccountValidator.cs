using Common;
using Common.Reflection;
using Common.Services;
using Common.Services.Validators;
using Services.AuthZ.Properties;
using Services.MessageContracts;
using ServiceStack.FluentValidation;

namespace Services.AuthZ.Validators
{
    partial class CreateUserAccountValidator
    {
        partial void AddRules()
        {
            RuleFor(dto => dto.Forenames).Matches(DataFormats.User.Forenames.Expression)
                .WithMessage(Resources.CreateUserAccountValidator_InvalidForenames);
            RuleFor(dto => dto.Surname).Matches(DataFormats.User.Surname.Expression)
                .WithMessage(Resources.CreateUserAccountValidator_InvalidSurname);
            RuleFor(dto => dto.Address).NotNull()
                .WithMessage(Resources.CreateUserAccountValidator_InvalidAddress);
            When(dto => dto.Address != null, () =>
            {
                RuleFor(dto => dto.Address)
                    .SetValidator(new AddressValidator(Reflector<CreateUserAccount>.GetPropertyName(x => x.Address)));
            });
            When(dto => dto.MobilePhone.HasValue(), () =>
            {
                RuleFor(dto => dto.MobilePhone).Matches(DataFormats.User.Phone.Expression)
                    .WithMessage(Resources.CreateUserAccountValidator_InvalidMobilePhone);
            });
            RuleFor(dto => dto.Email).EmailAddress()
                .WithMessage(Resources.CreateUserAccountValidator_InvalidEmail);

            Unless(dto => !dto.Username.HasValue()
                          && !dto.PasswordHash.HasValue(), () =>
                          {
                              RuleFor(dto => dto.Username).NotEmpty()
                                  .WithMessage(Resources.CreateUserAccountValidator_InvalidUsername);
                              RuleFor(dto => dto.Username).IsUsername()
                                  .WithMessage(Resources.CreateUserAccountValidator_InvalidUsername);
                              RuleFor(dto => dto.PasswordHash).NotEmpty()
                                  .WithMessage(Resources.CreateUserAccountValidator_InvalidPasswordHash);
                              RuleFor(dto => dto.PasswordHash).IsPasswordHash()
                                  .WithMessage(Resources.CreateUserAccountValidator_InvalidPasswordHash);
                          });
        }
    }
}