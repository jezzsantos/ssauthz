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
    public class UpdateUserAccountValidatorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private UpdateUserAccount dto;
            private UpdateUserAccountValidator validator;

            [TestInitialize]
            public void Initialize()
            {
                validator = new UpdateUserAccountValidator();

                dto = new UpdateUserAccount
                {
                    Id = Guid.NewGuid().ToString(),
                    Address = new Address
                    {
                        Street1 = "astreet1",
                        Street2 = "astreet2",
                        Town = "atown",
                    },
                    Email = "user@foo.com",
                    MobilePhone = "+123-045-67 89 10",
                    Forenames = "forenames",
                    OldPasswordHash = PasswordHasher.CreateHash("apassword"),
                    NewPasswordHash = PasswordHasher.CreateHash("apassword"),
                    Surname = "asurname",
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllValid_ThenSucceeds()
            {
                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIdBlank_ThenThrows()
            {
                dto.Id = string.Empty;

                Assert.Throws<ValidationException>(FluentValidation<UpdateUserAccount>.NotEmptyErrorMessage(x => x.Id),
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIdInvalid_ThenThrows()
            {
                dto.Id = "foo";

                Assert.Throws<ValidationException>(Resources.UpdateUserAccountValidator_InvalidId,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenForenamesBlank_ThenSucceeds()
            {
                dto.Forenames = string.Empty;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenForenamesInvalid_ThenThrows()
            {
                dto.Forenames = "a";

                Assert.Throws<ValidationException>(Resources.UpdateUserAccountValidator_InvalidForenames,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSurnameBlank_ThenSucceeds()
            {
                dto.Surname = string.Empty;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSurnameInvalid_ThenThrows()
            {
                dto.Surname = "a";

                Assert.Throws<ValidationException>(Resources.UpdateUserAccountValidator_InvalidSurname,
                    () => validator.ValidateAndThrow(dto));
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
                dto.MobilePhone = "a";

                Assert.Throws<ValidationException>(Resources.UpdateUserAccountValidator_InvalidMobilePhone,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEmailBlank_ThenSucceeds()
            {
                dto.Email = string.Empty;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEmailInvalid_ThenThrows()
            {
                dto.Email = "a";

                Assert.Throws<ValidationException>(Resources.UpdateUserAccountValidator_InvalidEmail,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddressNull_ThenSucceeds()
            {
                dto.Address = null;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenOldPasswordHashBlank_ThenSucceeds()
            {
                dto.OldPasswordHash = string.Empty;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNewPasswordHashBlank_ThenSucceeds()
            {
                dto.NewPasswordHash = string.Empty;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNewPasswordHashInvalid_ThenThrows()
            {
                dto.NewPasswordHash = "foo";

                Assert.Throws<ValidationException>(Resources.UpdateUserAccountValidator_InvalidNewPasswordHash,
                    () => validator.ValidateAndThrow(dto));
            }
        }
    }
}