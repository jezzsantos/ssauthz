using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack;
using Testing.Common;

namespace Services.UnitTests.Common
{
    public class StringExtensionsSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAString
        {
            private string value;

            [TestInitialize]
            public void Initialize()
            {
                this.value = null;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenHasValue_ThenReturns()
            {
                Assert.False(this.value.HasValue());

                this.value = "foo";
                Assert.True(this.value.HasValue());

                this.value = "";
                Assert.False(this.value.HasValue());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEqualsIgnoreCase_ThenReturns()
            {
                this.value = "Abc";

                Assert.True("Abc".EqualsIgnoreCase(this.value));
                Assert.True("abc".EqualsIgnoreCase(this.value));
                Assert.True("ABC".EqualsIgnoreCase(this.value));

                Assert.False("x".EqualsIgnoreCase(this.value));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNotEqualsIgnoreCase_ThenReturns()
            {
                this.value = "Abc";

                Assert.False("Abc".NotEqualsIgnoreCase(this.value));
                Assert.False("abc".NotEqualsIgnoreCase(this.value));
                Assert.False("ABC".NotEqualsIgnoreCase(this.value));

                Assert.True("x".NotEqualsIgnoreCase(this.value));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMakeCamel_ThenReturns()
            {
                Assert.Equal("apples", "Apples".MakeCamel());
                Assert.Equal("aPPLES", "APPLES".MakeCamel());
                Assert.Equal("apples.Bananas", "Apples.Bananas".MakeCamel());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsFormattedFrom_ThenReturns()
            {
                Assert.True("Apples and {0} Bananas".IsFormattedFrom(""));

                Assert.True("Apples and 4 Bananas".IsFormattedFrom("Apples and {0} Bananas"));
                Assert.False("Apples and Bananas".IsFormattedFrom("Apples and {0} Bananas"));
                Assert.False("Apples and".IsFormattedFrom("Apples and {0} Bananas"));

                Assert.False("Apples and 4 Bananas".IsFormattedFrom("Picnics and bears"));
            }
        }
    }
}
