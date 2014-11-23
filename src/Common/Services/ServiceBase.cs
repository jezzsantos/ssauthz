using System;
using System.Diagnostics;
using System.Net;
using ServiceStack;

namespace Common.Services
{
    /// <summary>
    ///     A base class for all services
    /// </summary>
    public abstract class ServiceBase : Service
    {
        /// <summary>
        ///     Sets the HTTP status code to return to client.
        /// </summary>
        internal void SetResponseCode(HttpStatusCode httpStatusCode)
        {
            if (Response != null)
            {
                Response.StatusCode = (int) httpStatusCode;
            }
        }

        /// <summary>
        ///     Adds a Location header to the response, using the request URL plus the resource Identifier
        /// </summary>
        protected void SetLocationHeader(string resourceId)
        {
            if (Response != null)
            {
                if (resourceId.HasValue())
                {
                    string url = @"{0}/{1}".FormatWith(Request.AbsoluteUri, resourceId);
                    Response.AddHeader(HttpHeaders.Location, url);
                }
            }
        }

        /// <summary>
        ///     Handles the specified action, and throws appropriate exception
        /// </summary>
        [DebuggerStepThrough]
        protected TResponse ProcessRequest<TResponse>(IReturn<TResponse> request, HttpStatusCode code,
            Func<TResponse> action) where TResponse : class
        {
            string requestType = (request != null) ? request.GetType().Name : string.Empty;
            //TODO: Audit this call in your analytics

            try
            {
                Guard.NotNull(() => request, request);

                TResponse response = null;
                response = action();

                if (response == null)
                {
                    throw new NotSupportedException();
                }

                SetResponseCode(code);

                //TODO: Audit this response in your analytics

                return response;
            }
            catch (ArgumentException ex)
            {
                throw HttpErrorThrower.BadRequest(ex.Message);
            }
            catch (ResourceNotFoundException ex)
            {
                throw HttpErrorThrower.NotFound(ex.Message);
            }
            catch (ResourceConflictException ex)
            {
                throw HttpErrorThrower.Conflict(ex.Message);
            }
            catch (RuleViolationException ex)
            {
                throw HttpErrorThrower.BadRequest(ex.Message);
            }
            catch (Exception)
            {
                throw HttpErrorThrower.InternalServerError();
            }
        }
    }
}