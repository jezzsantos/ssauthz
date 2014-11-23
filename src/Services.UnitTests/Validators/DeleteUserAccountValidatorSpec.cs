using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Validators;
using Services.MessageContracts;
using ServiceStack.FluentValidation;
using Testing.Common;

namespace Services.UnitTests.Validators
{
    public class DeleteUserAccountValidatorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private DeleteUserAccountValidator validator;

            [TestInitialize]
            public void Initialize()
            {
                validator = new DeleteUserAccountValidator();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIdIsNull_ThenThrows()
            {
                var dto = new DeleteUserAccount
                {
                    Id = null,
                };

                Assert.Throws<ValidationException>(FluentValidation<DeleteUserAccount>.NotEmptyErrorMessage(x => x.Id),
                    () => validator.ValidateAndThrow(dto));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllPropertiesValid_ThenSucceeds()
            {
                var dto = new DeleteUserAccount
                {
                    Id = "avalue",
                };

                validator.ValidateAndThrow(dto);
            }
        }
    }
}