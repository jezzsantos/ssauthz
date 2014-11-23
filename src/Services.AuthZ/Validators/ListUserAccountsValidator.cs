using Common;
using Common.Services.Validators;
using Services.AuthZ.Properties;
using ServiceStack.FluentValidation;

namespace Services.AuthZ.Validators
{
    partial class ListUserAccountsValidator
    {
        partial void AddRules()
        {
            When(dto => dto.Username.HasValue(), () =>
            {
                RuleFor(dto => dto.Username).IsUsername()
                    .WithMessage(Resources.ListUserAccountsValidator_InvalidUsername);
            });
            When(dto => dto.Email.HasValue(), () =>
            {
                RuleFor(dto => dto.Email).EmailAddress()
                    .WithMessage(Resources.ListUserAccountsValidator_InvalidEmail);
            });
        }
    }
}