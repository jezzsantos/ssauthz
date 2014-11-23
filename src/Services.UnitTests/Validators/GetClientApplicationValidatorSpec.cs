using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Validators;
using Services.MessageContracts;
using ServiceStack.FluentValidation;
using Testing.Common;

namespace Services.UnitTests.Validators
{
    public class GetClientApplicationValidatorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private GetClientApplicationValidator validator;

            [TestInitialize]
            public void Initialize()
            {
                validator = new GetClientApplicationValidator();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIdIsNull_ThenThrows()
            {
                var dto = new GetClientApplication
                {
                    Id = null,
                };

                Assert.Throws<ValidationException>(
                    FluentValidation<GetClientApplication>.NotEmptyErrorMessage(x => x.Id),
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllPropertiesValid_ThenSucceeds()
            {
                var dto = new GetClientApplication
                {
                    Id = "avalue",
                };

                validator.ValidateAndThrow(dto);
            }
        }
    }
}