using Common;
using Common.Security;
using Common.Services;
using Common.Services.Validators;
using Services.AuthZ.Properties;
using ServiceStack;
using ServiceStack.FluentValidation;

namespace Services.AuthZ.Validators
{
    partial class CreateAccessTokenValidator
    {
        partial void AddRules()
        {
            When(dto => dto.GrantType.HasValue(), () =>
            {
                RuleFor(dto => dto.GrantType).Must(x =>
                    x.EqualsIgnoreCase(GrantTypes.AccessToken)
                    || x.EqualsIgnoreCase(GrantTypes.RefreshToken))
                    .WithMessage(Resources.CreateAccessTokenValidator_InvalidGrantType);

                When(dto => dto.GrantType.EqualsIgnoreCase(GrantTypes.AccessToken), () =>
                {
                    RuleFor(dto => dto.Username).NotEmpty()
                        .WithMessage(Resources.CreateAccessTokenValidator_InvalidUsername);
                    When(dto => dto.Username.HasValue(), () =>
                    {
                        RuleFor(x => x.Username).IsUsername()
                            .WithMessage(Resources.CreateAccessTokenValidator_InvalidUsername);
                    });
                    RuleFor(dto => dto.Password).NotEmpty()
                        .WithMessage(Resources.CreateAccessTokenValidator_InvalidPassword);
                    RuleFor(dto => dto.Password).Matches(DataFormats.User.Password.Expression)
                        .WithMessage(Resources.CreateAccessTokenValidator_InvalidPassword);
                    RuleFor(dto => dto.Scope).NotEmpty()
                        .WithMessage(Resources.CreateAccessTokenValidator_InvalidScope);
                });
                When(dto => dto.GrantType.EqualsIgnoreCase(GrantTypes.RefreshToken), () =>
                {
                    RuleFor(dto => dto.RefreshToken).NotEmpty()
                        .WithMessage(Resources.CreateAccessTokenValidator_InvalidRefreshToken);
                });
            });
        }
    }
}