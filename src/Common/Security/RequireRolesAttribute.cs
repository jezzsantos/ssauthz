using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using ServiceStack;
using ServiceStack.Web;

namespace Common.Security
{
    /// <summary>
    ///     Define an attribute for securing REST calls to ensure the authenticated user is in the authorized roles
    /// </summary>
    public class RequireRolesAttribute : RequestFilterAttribute
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="RequireRolesAttribute" /> class.
        /// </summary>
        public RequireRolesAttribute(params string[] roles)
            : this(ApplyTo.All, roles)
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="RequireRolesAttribute" /> class.
        /// </summary>
        public RequireRolesAttribute(ApplyTo applyTo, params string[] roles)
            : base(applyTo)
        {
            Guard.NotNull(() => roles, roles);

            AllRoles = roles;
            Priority = -90;
        }

        /// <summary>
        ///     Gets or sets the roles for the current user.
        /// </summary>
        public IEnumerable<string> AllRoles { get; set; }

        /// <summary>
        ///     Executes the attribute
        /// </summary>
        public override void Execute(IRequest request, IResponse response, object requestDto)
        {
            IPrincipal currentUser = ((HttpRequestBase) request.OriginalRequest).RequestContext.HttpContext.User;

            // Check for god role
            if (currentUser.IsInRole(AuthorizationRoles.God))
            {
                return;
            }

            // Check user is in any role
            if (AllRoles.Any()
                && HasAnyRole(currentUser, AllRoles))
            {
                return;
            }

            response.StatusCode = (int) HttpStatusCode.Unauthorized;
            response.EndRequest();
        }

        private static bool HasAnyRole(IPrincipal user, IEnumerable<string> roles)
        {
            return (roles.Any(r => user.IsInRole(r)));
        }
    }
}