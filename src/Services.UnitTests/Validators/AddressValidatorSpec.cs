using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Properties;
using Services.AuthZ.Validators;
using Services.DataContracts;
using ServiceStack.FluentValidation;
using Testing.Common;

namespace Services.UnitTests.Validators
{
    public class AddressValidatorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private Address dto;
            private AddressValidator validator;

            [TestInitialize]
            public void Initialize()
            {
                validator = new AddressValidator("Address");
                dto = new Address
                {
                    Street1 = "astreet1",
                    Street2 = "astreet2",
                    Town = "atown",
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllValid_ThenSucceeds()
            {
                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenStreet1IsEmpty_ThenSucceeds()
            {
                dto.Street1 = string.Empty;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenStreet1Invalid_ThenThrows()
            {
                dto.Street1 = "1";

                Assert.Throws<ValidationException>(Resources.AddressValidator_InvalidAddressStreet,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenStreet2IsEmpty_ThenSucceeds()
            {
                dto.Street2 = string.Empty;

                validator.ValidateAndThrow(dto);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenStreet2Invalid_ThenThrows()
            {
                dto.Street2 = "1";

                Assert.Throws<ValidationException>(Resources.AddressValidator_InvalidAddressStreet,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTownEmpty_ThenThrows()
            {
                dto.Town = string.Empty;

                Assert.Throws<ValidationException>(Resources.AddressValidator_InvalidAddressTown,
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTownInvalid_ThenThrows()
            {
                dto.Town = "1";

                Assert.Throws<ValidationException>(Resources.AddressValidator_InvalidAddressTown,
                    () => validator.ValidateAndThrow(dto));
            }
        }
    }
}