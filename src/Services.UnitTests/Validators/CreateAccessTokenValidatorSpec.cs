using Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Properties;
using Services.AuthZ.Validators;
using Services.MessageContracts;
using ServiceStack.FluentValidation;
using Testing.Common;

namespace Services.UnitTests.Validators
{
    public class CreateAccessTokenValidatorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private CreateAccessToken dto;
            private CreateAccessTokenValidator validator;

            [TestInitialize]
            public void Initialize()
            {
                validator = new CreateAccessTokenValidator();
                dto = new CreateAccessToken
                {
                    GrantType = GrantTypes.AccessToken,
                    Password = "apassword",
                    Username = "ausername",
                    RefreshToken = "arefreshtoken",
                    Scope = "ascope",
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllPropertiesValid_ThenThrows()
            {
                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGrantTypeIsNull_ThenThrows()
            {
                dto.GrantType = null;

                Assert.Throws<ValidationException>(
                    FluentValidation<CreateAccessToken>.NotNullErrorMessage(x => x.GrantType),
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGrantTypeIsAccessTokenAndUsernameIsNull_ThenThrows()
            {
                dto.GrantType = GrantTypes.AccessToken;
                dto.Username = null;

                Assert.Throws<ValidationException>(Resources.CreateAccessTokenValidator_InvalidUsername,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGrantTypeIsAccessTokenAndUsernameIsInvalid_ThenThrows()
            {
                dto.GrantType = GrantTypes.AccessToken;
                dto.Username = "x";

                Assert.Throws<ValidationException>(Resources.CreateAccessTokenValidator_InvalidUsername,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGrantTypeIsAccessTokenAndUsernameIsAUsername_ThenSuccess()
            {
                dto.GrantType = GrantTypes.AccessToken;
                dto.Username = "ausername";

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGrantTypeIsAccessTokenAndPasswordIsNull_ThenThrows()
            {
                dto.GrantType = GrantTypes.AccessToken;
                dto.Password = null;

                Assert.Throws<ValidationException>(Resources.CreateAccessTokenValidator_InvalidPassword,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGrantTypeIsAccessTokenAndPasswordIsInvalid_ThenThrows()
            {
                dto.GrantType = GrantTypes.AccessToken;
                dto.Password = "x";

                Assert.Throws<ValidationException>(Resources.CreateAccessTokenValidator_InvalidPassword,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGrantTypeIsAccessTokenAndScopeIsNull_ThenThrows()
            {
                dto.GrantType = GrantTypes.AccessToken;
                dto.Scope = null;

                Assert.Throws<ValidationException>(Resources.CreateAccessTokenValidator_InvalidScope,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGrantTypeIsAccessTokenWithCredentialsAndScope_ThenSucceeds()
            {
                dto.GrantType = GrantTypes.AccessToken;
                dto.Scope = "ascope";
                dto.Username = "ausername";
                dto.Password = "apassword";

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGrantTypeIsRefreshTokenAndRefreshTokenIsNull_ThenThrows()
            {
                dto.GrantType = GrantTypes.RefreshToken;
                dto.RefreshToken = null;

                Assert.Throws<ValidationException>(Resources.CreateAccessTokenValidator_InvalidRefreshToken,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGrantTypeIsRefreshTokenAndRefreshToken_ThenSucceeds()
            {
                dto.GrantType = GrantTypes.RefreshToken;
                dto.RefreshToken = "arefreshtoken";

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGrantTypeIsNeitherPasswordNorRefreshToken_ThenThrows()
            {
                dto.GrantType = "foo";

                Assert.Throws<ValidationException>(Resources.CreateAccessTokenValidator_InvalidGrantType,
                    () => validator.ValidateAndThrow(dto));
            }
        }
    }
}