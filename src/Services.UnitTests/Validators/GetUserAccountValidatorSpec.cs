using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Validators;
using Services.MessageContracts;
using ServiceStack.FluentValidation;
using Testing.Common;

namespace Services.UnitTests.Validators
{
    public class GetUserAccountValidatorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private GetUserAccountValidator validator;

            [TestInitialize]
            public void Initialize()
            {
                validator = new GetUserAccountValidator();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIdIsNull_ThenThrows()
            {
                var dto = new GetUserAccount
                {
                    Id = null,
                };

                Assert.Throws<ValidationException>(FluentValidation<GetUserAccount>.NotEmptyErrorMessage(x => x.Id),
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllPropertiesValid_ThenSucceeds()
            {
                var dto = new GetUserAccount
                {
                    Id = "avalue",
                };

                validator.ValidateAndThrow(dto);
            }
        }
    }
}