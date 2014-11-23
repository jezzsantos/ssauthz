using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Properties;
using Services.AuthZ.Validators;
using Services.MessageContracts;
using ServiceStack.FluentValidation;
using Testing.Common;

namespace Services.UnitTests.Validators
{
    public class ListUserAccountsValidatorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private ListUserAccounts dto;
            private ListUserAccountsValidator validator;

            [TestInitialize]
            public void Initialize()
            {
                validator = new ListUserAccountsValidator();

                dto = new ListUserAccounts
                {
                    Email = "user@foo.com",
                    Username = "ausername",
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllValid_ThenSucceeds()
            {
                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEmailEmptyAndUsername_ThenSucceeds()
            {
                dto.Email = string.Empty;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUsernameEmptyAndValidEmail_ThenSucceeds()
            {
                dto.Username = string.Empty;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUsernameEmptyAndInValidEmail_ThenThrows()
            {
                dto.Username = string.Empty;
                dto.Email = "foo";

                Assert.Throws<ValidationException>(Resources.ListUserAccountsValidator_InvalidEmail,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUsernameAndInValidEmail_ThenThrows()
            {
                dto.Username = "ausername";
                dto.Email = "foo";

                Assert.Throws<ValidationException>(Resources.ListUserAccountsValidator_InvalidEmail,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUsernameEmptyAndEmailEmpty_ThenSucceeds()
            {
                dto.Username = string.Empty;
                dto.Email = string.Empty;

                validator.ValidateAndThrow(dto);
            }
        }
    }
}