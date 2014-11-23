using System;
using System.Collections.Generic;
using System.Net;
using ServiceStack;

namespace Common.Services
{
    /// <summary>
    ///     Error codes for common errors thrown from services
    /// </summary>
    public static class HttpErrorCode
    {
        private const string ResourceConflict = "Conflict";
        private const string ResourceNotFound = "Not Found";
        private const string BadRequest = "Bad Request";
        private const string InternalServerError = "Internal Server Error";
        private const string NotImplemented = "Not Implemented";
        private const string Unauthorized = "Unauthorized";
        private const string Forbidden = "Forbidden";

        private static readonly Dictionary<HttpStatusCode, string> CodeDescriptions = new Dictionary
            <HttpStatusCode, string>
        {
            {HttpStatusCode.Conflict, ResourceConflict},
            {HttpStatusCode.NotFound, ResourceNotFound},
            {HttpStatusCode.BadRequest, BadRequest},
            {HttpStatusCode.InternalServerError, InternalServerError},
            {HttpStatusCode.NotImplemented, NotImplemented},
            {HttpStatusCode.Unauthorized, Unauthorized},
            {HttpStatusCode.Forbidden, Forbidden},
        };

        public static string FromHttpStatusCode(HttpStatusCode httpStatusCode)
        {
            return CodeDescriptions[httpStatusCode];
        }
    }

    /// <summary>
    ///     Helper class for throwing HTTP errors
    /// </summary>
    public static class HttpErrorThrower
    {
        /// <summary>
        ///     Throws a HTTP 400 bad request
        /// </summary>
        public static Exception BadRequest(string message = null)
        {
            string formattedMessage = (message.HasValue()
                ? message
                : HttpErrorCode.FromHttpStatusCode(HttpStatusCode.BadRequest));
            return new HttpError(HttpStatusCode.BadRequest, HttpErrorCode.FromHttpStatusCode(HttpStatusCode.BadRequest),
                formattedMessage);
        }

        /// <summary>
        ///     Throws a HTTP 401 unauthorized
        /// </summary>
        public static Exception Unauthorized(string message = null)
        {
            string formattedMessage = (message.HasValue()
                ? message
                : HttpErrorCode.FromHttpStatusCode(HttpStatusCode.Unauthorized));
            return new HttpError(HttpStatusCode.Unauthorized,
                HttpErrorCode.FromHttpStatusCode(HttpStatusCode.Unauthorized), formattedMessage);
        }

        /// <summary>
        ///     Throws a HTTP 403 forbidden
        /// </summary>
        public static Exception Forbidden(string message = null)
        {
            string formattedMessage = (message.HasValue()
                ? message
                : HttpErrorCode.FromHttpStatusCode(HttpStatusCode.Forbidden));
            return new HttpError(HttpStatusCode.Forbidden,
                HttpErrorCode.FromHttpStatusCode(HttpStatusCode.Forbidden), formattedMessage);
        }

        /// <summary>
        ///     Throws a HTTP 404 error
        /// </summary>
        public static Exception NotFound(string message = null)
        {
            string formattedMessage = (message.HasValue()
                ? message
                : HttpErrorCode.FromHttpStatusCode(HttpStatusCode.NotFound));
            return new HttpError(HttpStatusCode.NotFound, HttpErrorCode.FromHttpStatusCode(HttpStatusCode.NotFound),
                formattedMessage);
        }

        /// <summary>
        ///     Throws a HTTP 409 error
        /// </summary>
        public static Exception Conflict(string message = null)
        {
            string formattedMessage = (message.HasValue()
                ? message
                : HttpErrorCode.FromHttpStatusCode(HttpStatusCode.Conflict));
            return new HttpError(HttpStatusCode.Conflict, HttpErrorCode.FromHttpStatusCode(HttpStatusCode.Conflict),
                formattedMessage);
        }

        /// <summary>
        ///     Throws a HTTP 500 internal server error
        /// </summary>
        public static Exception InternalServerError(string message = null)
        {
            string formattedMessage = (message.HasValue()
                ? message
                : HttpErrorCode.FromHttpStatusCode(HttpStatusCode.InternalServerError));
            return new HttpError(HttpStatusCode.InternalServerError,
                HttpErrorCode.FromHttpStatusCode(HttpStatusCode.InternalServerError), formattedMessage);
        }

        /// <summary>
        ///     Throws a HTTP 501 not implemented
        /// </summary>
        public static Exception NotImplemented(string message = null)
        {
            string formattedMessage = (message.HasValue()
                ? message
                : HttpErrorCode.FromHttpStatusCode(HttpStatusCode.NotImplemented));
            return new HttpError(HttpStatusCode.NotImplemented,
                HttpErrorCode.FromHttpStatusCode(HttpStatusCode.NotImplemented), formattedMessage);
        }
    }
}