using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Validators;
using Services.MessageContracts;
using ServiceStack.FluentValidation;
using Testing.Common;

namespace Services.UnitTests.Validators
{
    public class CreateClientApplicationValidatorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private CreateClientApplicationValidator validator;

            [TestInitialize]
            public void Initialize()
            {
                validator = new CreateClientApplicationValidator();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNameIsNull_ThenThrows()
            {
                var dto = new CreateClientApplication
                {
                    Name = null,
                };

                Assert.Throws<ValidationException>(
                    FluentValidation<CreateClientApplication>.NotNullErrorMessage(x => x.Name),
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllPropertiesValid_ThenSucceeds()
            {
                var dto = new CreateClientApplication
                {
                    Name = "avalue",
                };

                validator.ValidateAndThrow(dto);
            }
        }
    }
}