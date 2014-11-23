using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.AuthZ.Security;
using Testing.Common;

namespace Services.UnitTests.Security
{
    public class UserAuthInfoSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        private static readonly KeyValuePair<string, string> ExamplePasswordHash = new KeyValuePair<string, string>
            (
            "foo", @"sha1:1000:XbmqEUsz0/Tdm4z7b5qGry6GuNzwK/xs:mOJxbKFk1QCxqr/X0vWWLUIuvPUNcdT4"
            );

        [TestClass]
        public class GivenAContext
        {
            private UserAuthInfo account;

            [TestInitialize]
            public void Initialize()
            {
                account = new UserAuthInfo();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenVerifyPasswordWithNullPassword_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () => account.VerifyPassword(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenVerifyPasswordWithIncorrectPassword_ThenReturnsFalse()
            {
                account.PasswordHash = ExamplePasswordHash.Value;

                bool result = account.VerifyPassword("foo1");

                Assert.False(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenVerifyPasswordWithCorrectPassword_ThenReturnsTrue()
            {
                account.PasswordHash = ExamplePasswordHash.Value;

                bool result = account.VerifyPassword(ExamplePasswordHash.Key);

                Assert.True(result);
            }
        }
    }
}