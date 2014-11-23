using System.Web;
using ServiceStack.Web;

namespace Common.Services
{
    /// <summary>
    ///     Extensions ot the <see cref="IRequest" />
    /// </summary>
    public static class RequestExtensions
    {
        public static string GetCurrentUser(this IRequest request)
        {
            if (request != null)
            {
                return HttpContext.Current.User.Identity.Name;
            }

            return null;
        }
    }
}