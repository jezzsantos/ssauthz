using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Validators;
using Services.MessageContracts;
using ServiceStack.FluentValidation;
using Testing.Common;

namespace Services.UnitTests.Validators
{
    public class DeleteClientApplicationValidatorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private DeleteClientApplicationValidator validator;

            [TestInitialize]
            public void Initialize()
            {
                validator = new DeleteClientApplicationValidator();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIdIsNull_ThenThrows()
            {
                var dto = new DeleteClientApplication
                {
                    Id = null,
                };

                Assert.Throws<ValidationException>(
                    FluentValidation<DeleteClientApplication>.NotEmptyErrorMessage(x => x.Id),
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllPropertiesValid_ThenSucceeds()
            {
                var dto = new DeleteClientApplication
                {
                    Id = "avalue",
                };

                validator.ValidateAndThrow(dto);
            }
        }
    }
}