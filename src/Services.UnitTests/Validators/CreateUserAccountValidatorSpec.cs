using System;
using Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Properties;
using Services.AuthZ.Validators;
using Services.DataContracts;
using Services.MessageContracts;
using ServiceStack.FluentValidation;
using Testing.Common;

namespace Services.UnitTests.Validators
{
    public class CreateUserAccountValidatorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private CreateUserAccount dto;
            private CreateUserAccountValidator validator;

            [TestInitialize]
            public void Initialize()
            {
                validator = new CreateUserAccountValidator();

                dto = new CreateUserAccount
                {
                    Address = new Address
                    {
                        Street1 = "astreet1",
                        Street2 = "astreet2",
                        Town = "atown",
                    },
                    Email = "user@foo.com",
                    Forenames = "forenames",
                    Surname = "asurname",
                    MobilePhone = "+123-045-67 89 10",
                    Username = "ausername",
                    PasswordHash = PasswordHasher.CreateHash("apasswordhash"),
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllValid_ThenSucceeds()
            {
                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMobilePhoneBlank_ThenSucceeds()
            {
                dto.MobilePhone = string.Empty;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMobilePhoneInvalid_ThenThrows()
            {
                dto.MobilePhone = "sometext";

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidMobilePhone,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEmailBlank_ThenThrows()
            {
                dto.Email = string.Empty;

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidEmail,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEmailInvalid_ThenThrows()
            {
                dto.Email = "justname@";

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidEmail,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenForenamesBlank_ThenThrows()
            {
                dto.Forenames = string.Empty;

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidForenames,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenForenamesInvalid_ThenThrows()
            {
                dto.Forenames = "a";

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidForenames,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSurnameBlank_ThenThrows()
            {
                dto.Surname = string.Empty;

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidSurname,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSurnameInvalid_ThenThrows()
            {
                dto.Surname = "a";

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidSurname,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddressNull_ThenThrows()
            {
                dto.Address = null;

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidAddress,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUsernameInvalid_ThenThrows()
            {
                dto.Username = "foo";

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidUsername,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUsernameBlankAndPasswordHashBlankAndSignatureHasBlank_ThenSucceeds()
            {
                dto.Username = string.Empty;
                dto.PasswordHash = string.Empty;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUsernameValidAndPasswordHashBlank_ThenThrows()
            {
                dto.Username = "ausername";
                dto.PasswordHash = string.Empty;

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidPasswordHash,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPasswordValidAndUsernameBlank_ThenThrows()
            {
                dto.Username = String.Empty;
                dto.PasswordHash = PasswordHasher.CreateHash("apasswordhash");

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidUsername,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenPasswordValidAndUsernameInvalid_ThenThrows()
            {
                dto.Username = String.Empty;
                dto.PasswordHash = PasswordHasher.CreateHash("apasswordhash");

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidUsername,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUsernameValidAndPasswordHashInvalid_ThenThrows()
            {
                dto.Username = "ausername";
                dto.PasswordHash = "awrongpasswordhash";

                Assert.Throws<ValidationException>(Resources.CreateUserAccountValidator_InvalidPasswordHash,
                    () => validator.ValidateAndThrow(dto));
            }
        }
    }
}