using Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Common;

namespace Services.UnitTests.Security
{
    public class FilterAttributeSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAllAuthorizationFilters
        {
            [TestMethod, TestCategory("Unit")]
            public void ThenWillExecuteInCorrectOrder()
            {
                var requireAuthZ = new RequireAuthorizationAttribute();
                var requireAuthZRoles = new RequireRolesAttribute();

                Assert.True(-requireAuthZ.Priority > -requireAuthZRoles.Priority);
            }
        }
    }
}