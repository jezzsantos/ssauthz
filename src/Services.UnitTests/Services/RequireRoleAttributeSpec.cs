using System;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using Common.Security;
using Funq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceStack;
using ServiceStack.Web;
using Testing.Common;
using RequestContext = System.Web.Routing.RequestContext;

namespace Services.UnitTests.Services
{
    public class RequireRoleAttributeSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private static FakeAppHost appHost;

            [ClassInitialize]
            public static void InitialiseAllTests(TestContext context)
            {
                // Host TestService
                if (ServiceStackHost.Instance != null)
                {
                    ServiceStackHost.Instance.Dispose();
                }
                appHost = new FakeAppHost();
                appHost.Init();
            }

            [ClassCleanup]
            public static void CleanUpAllTests()
            {
                appHost.Stop();
                ServiceStackHost.Instance.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateNewWithNullRoles_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new RequireRolesAttribute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateNewWithEmptyRoles_ThenCreates()
            {
                new RequireRolesAttribute();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateWithNoApplyTo_ThenApplyToAll()
            {
                var attribute = new RequireRolesAttribute();

                Assert.Equal(ApplyTo.All, attribute.ApplyTo);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateWithRoles_ThenAllRolesAssigned()
            {
                var attribute = new RequireRolesAttribute("foo");

                Assert.Equal("foo", attribute.AllRoles.First());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithEmptyRoles_ThenUnauthorized()
            {
                var attribute = new RequireRolesAttribute();

                var response = new Mock<IResponse>();
                var request = GetRequestMock(new Mock<IPrincipal>());

                attribute.Execute(request.Object, response.Object, null);

                VerifyUnauthorizedResponse(response);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithUnknownRole_ThenUnauthorized()
            {
                var attribute = new RequireRolesAttribute("foo");

                var response = new Mock<IResponse>();
                var user = new Mock<IPrincipal>();
                user.Setup(usr => usr.IsInRole(It.IsAny<string>()))
                    .Returns(false);
                var request = GetRequestMock(user);

                attribute.Execute(request.Object, response.Object, null);

                VerifyUnauthorizedResponse(response);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithSingleKnownRole_ThenAuthorized()
            {
                var attribute = new RequireRolesAttribute("foo");

                var response = new Mock<IResponse>();
                var user = new Mock<IPrincipal>();
                user.Setup(usr => usr.IsInRole("foo"))
                    .Returns(true);
                var request = GetRequestMock(user);

                attribute.Execute(request.Object, response.Object, null);

                VerifyAuthorized(response);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithSingleUnknownRole_ThenUnauthorized()
            {
                var attribute = new RequireRolesAttribute("foo");

                var response = new Mock<IResponse>();
                var user = new Mock<IPrincipal>();
                user.Setup(usr => usr.IsInRole("foo"))
                    .Returns(false);
                var request = GetRequestMock(user);

                attribute.Execute(request.Object, response.Object, null);

                VerifyUnauthorizedResponse(response);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithAllKnownRoles_ThenAuthorized()
            {
                var attribute = new RequireRolesAttribute("foo", "bar");

                var response = new Mock<IResponse>();
                var user = new Mock<IPrincipal>();
                user.Setup(usr => usr.IsInRole(It.IsAny<string>()))
                    .Returns(true);
                var request = GetRequestMock(user);

                attribute.Execute(request.Object, response.Object, null);

                VerifyAuthorized(response);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithAnyRoles_ThenAuthorized()
            {
                var attribute = new RequireRolesAttribute("foo", "bar");

                var response = new Mock<IResponse>();
                var user = new Mock<IPrincipal>();
                user.Setup(usr => usr.IsInRole("foo"))
                    .Returns(true);
                user.Setup(usr => usr.IsInRole("bar"))
                    .Returns(false);
                var request = GetRequestMock(user);

                attribute.Execute(request.Object, response.Object, null);

                VerifyAuthorized(response);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithManyRolesAndUserIsGod_ThenAuthorized()
            {
                var attribute = new RequireRolesAttribute("foo", "bar");

                var response = new Mock<IResponse>();
                var user = new Mock<IPrincipal>();
                user.Setup(usr => usr.IsInRole(AuthorizationRoles.God))
                    .Returns(true);
                user.Setup(usr => usr.IsInRole("foo"))
                    .Returns(false);
                user.Setup(usr => usr.IsInRole("bar"))
                    .Returns(false);
                var request = GetRequestMock(user);

                attribute.Execute(request.Object, response.Object, null);

                VerifyAuthorized(response);
            }

            private static Mock<IRequest> GetRequestMock(Mock<IPrincipal> principal)
            {
                var request = new Mock<IRequest>();
                var requestBase = new Mock<HttpRequestBase>();
                var requestContext = new Mock<RequestContext>();
                var httpContext = new Mock<HttpContextBase>();

                httpContext.Setup(hc => hc.User).Returns(principal.Object);
                requestContext.Setup(rc => rc.HttpContext).Returns(httpContext.Object);
                requestBase.Setup(rb => rb.RequestContext).Returns(requestContext.Object);
                request.Setup(r => r.OriginalRequest).Returns(requestBase.Object);

                return request;
            }

            private static void VerifyUnauthorizedResponse(Mock<IResponse> response)
            {
                response.VerifySet(r => r.StatusCode = (int)HttpStatusCode.Unauthorized);
                response.Verify(r => r.Close(), Times.Once());
            }

            private static void VerifyAuthorized(Mock<IResponse> response)
            {
                response.Verify(r => r.StatusCode, Times.Never());
                response.Verify(r => r.Close(), Times.Never());
            }
        }

        public class FakeAppHost : AppSelfHostBase
        {
            public FakeAppHost()
                : base("FakeAppHost", typeof(FakeAppHost).Assembly)
            {
            }

            public override void Configure(Container container)
            {
            }
        }
    }
}
