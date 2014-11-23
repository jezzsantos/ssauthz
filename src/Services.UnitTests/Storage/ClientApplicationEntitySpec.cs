using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Storage.DataEntities;
using Testing.Common;

namespace Services.UnitTests.Storage
{
    public class ClientApplicationEntitySpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private ClientApplicationEntity entity;

            [TestInitialize]
            public void Initialize()
            {
                entity = new ClientApplicationEntity();
            }

            //[TestMethod, TestCategory("Unit")]
            //public void WhenFromDto_ThenReturnsEntity()
            //{
            //    Assert.Fail("Not Implemented!");
            //}

            //[TestMethod, TestCategory("Unit")]
            //public void WhenToDto_ThenReturnsDto()
            //{
            //    Assert.Fail("Not Implemented!");
            //}
        }
    }
}