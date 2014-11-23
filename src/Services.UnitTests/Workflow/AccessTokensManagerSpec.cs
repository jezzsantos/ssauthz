using System;
using System.Collections.Specialized;
using Common.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services.AuthZ.Security;
using Services.AuthZ.Workflow;
using Services.MessageContracts;
using ServiceStack;
using ServiceStack.Web;
using Testing.Common;

namespace Services.UnitTests.Workflow
{
    public class AccessTokensManagerSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAJsonRequest
        {
            private Mock<IDnoaAuthZRequestProvider> dnoaAuthZProvider;
            private AccessTokensManager manager;
            private Mock<IRequest> request;

            [TestInitialize]
            public void Initialize()
            {
                dnoaAuthZProvider = new Mock<IDnoaAuthZRequestProvider>();
                manager = new AccessTokensManager
                {
                    DnoaAuthorizationServer = dnoaAuthZProvider.Object,
                };

                request = new Mock<IRequest>();
                request.Setup(req => req.ContentType).Returns(MimeTypes.Json);
                request.Setup(req => req.AbsoluteUri).Returns("https://foo.com");
                request.Setup(req => req.Headers)
                    .Returns(new NameValueCollectionWrapper(new NameValueCollection()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateAccessTokenWithNullRequest_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    manager.CreateAccessToken(null, new CreateAccessToken()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateAccessTokenWithNullBody_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    manager.CreateAccessToken(Mock.Of<IRequest>(), null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateAccessTokenAndProviderThrows_ThenThrows()
            {
                dnoaAuthZProvider.Setup(
                    pro => pro.HandleTokenRequest(It.IsAny<IRequest>(), It.IsAny<CreateAccessToken>()))
                    .Throws<RuleViolationException>();

                Assert.Throws<RuleViolationException>(() =>
                    manager.CreateAccessToken(request.Object, new CreateAccessToken()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateAccessToken_ThenReturnsAccessToken()
            {
                dnoaAuthZProvider.Setup(
                    pro => pro.HandleTokenRequest(It.IsAny<IRequest>(), It.IsAny<CreateAccessToken>()))
                    .Returns(new DnoaAuthZResponse
                    {
                        AccessToken = "anaccesstoken",
                        ExpiresIn = "anexpiresin",
                        RefreshToken = "arefreshtoken",
                        Scope = "ascope",
                        TokenType = "atokentype",
                    });

                CreateAccessTokenResponse result = manager.CreateAccessToken(request.Object, new CreateAccessToken());

                Assert.Equal("anaccesstoken", result.AccessToken);
                Assert.Equal("anexpiresin", result.ExpiresIn);
                Assert.Equal("arefreshtoken", result.RefreshToken);
                Assert.Equal("ascope", result.Scope);
                Assert.Equal("atokentype", result.TokenType);
            }
        }
    }
}