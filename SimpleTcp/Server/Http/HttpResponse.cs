using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTcp.Server.Http
{
    public class HttpResponse : IHttpResponse
    {
        #region Static Methods
        
        #endregion

        #region Properties
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public byte[] Content { get; set; }
        public HttpHeaders Headers { get; private set; }
        #endregion

        #region Public Methods
        public HttpResponse()
        {
            Headers = new HttpHeaders();
        }

        public HttpResponse(HttpStatusCode httpStatusCode) : this()
        {
            StatusCode = (int)httpStatusCode;
            #region SetReasonPhrase
            /// The reason phrases listed here are only recommendations
            switch (httpStatusCode)
            {
                case HttpStatusCode.Continue:
                    ReasonPhrase = "Continue";
                    break;
                case HttpStatusCode.SwitchingProtocols:
                    ReasonPhrase = "Switching Protocols";
                    break;

                case HttpStatusCode.OK:
                    ReasonPhrase = "OK";
                    break;
                case HttpStatusCode.Created:
                    ReasonPhrase = "Created";
                    break;
                case HttpStatusCode.Accepted:
                    ReasonPhrase = "Accepted";
                    break;
                case HttpStatusCode.NonAuthoritativeInformation:
                    ReasonPhrase = "Non-Authoritative Information";
                    break;
                case HttpStatusCode.NoContent:
                    ReasonPhrase = "No Content";
                    break;
                case HttpStatusCode.ResetContent:
                    ReasonPhrase = "Reset Content";
                    break;
                case HttpStatusCode.PartialContent:
                    ReasonPhrase = "Partial Content";
                    break;

                case HttpStatusCode.MultipleChoices:
                    ReasonPhrase = "Multiple Choices";
                    break;
                case HttpStatusCode.MovedPermanently:
                    ReasonPhrase = "Moved Permanently";
                    break;
                case HttpStatusCode.Found:
                    ReasonPhrase = "Found";
                    break;
                case HttpStatusCode.SeeOther:
                    ReasonPhrase = "See Other";
                    break;
                case HttpStatusCode.NotModified:
                    ReasonPhrase = "Not Modified";
                    break;
                case HttpStatusCode.UseProxy:
                    ReasonPhrase = "Use Proxy";
                    break;
                case HttpStatusCode.TemporaryRedirect:
                    ReasonPhrase = "Temporary Redirect";
                    break;

                case HttpStatusCode.BadRequest:
                    ReasonPhrase = "Bad Request";
                    break;
                case HttpStatusCode.Unauthorized:
                    ReasonPhrase = "Unauthorized";
                    break;
                case HttpStatusCode.PaymentRequired:
                    ReasonPhrase = "Payment Required";
                    break;
                case HttpStatusCode.Forbidden:
                    ReasonPhrase = "Forbidden";
                    break;
                case HttpStatusCode.NotFound:
                    ReasonPhrase = "Not Found";
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    ReasonPhrase = "Method Not Allowed";
                    break;
                case HttpStatusCode.NotAcceptable:
                    ReasonPhrase = "Not Acceptable";
                    break;

                case HttpStatusCode.ProxyAuthenticationRequired:
                    ReasonPhrase = "Proxy Authentication Required";
                    break;
                case HttpStatusCode.RequestTimeout:
                    ReasonPhrase = "Request Time-out";
                    break;
                case HttpStatusCode.Conflict:
                    ReasonPhrase = "Conflict";
                    break;
                case HttpStatusCode.Gone:
                    ReasonPhrase = "Gone";
                    break;
                case HttpStatusCode.LengthRequired:
                    ReasonPhrase = "Length Required";
                    break;
                case HttpStatusCode.PreconditionFailed:
                    ReasonPhrase = "Precondition Failed";
                    break;
                case HttpStatusCode.RequestEntityTooLarge:
                    ReasonPhrase = "Request Entity Too Large";
                    break;
                case HttpStatusCode.RequestURITooLarge:
                    ReasonPhrase = "Request-URI Too Large";
                    break;
                case HttpStatusCode.UnsupportedMediaType:
                    ReasonPhrase = "Unsupported Media Type";
                    break;
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                    ReasonPhrase = "Requested range not satisfiable";
                    break;
                case HttpStatusCode.ExpectationFailed:
                    ReasonPhrase = "Expectation Failed";
                    break;

                case HttpStatusCode.InternalServerError:
                    ReasonPhrase = "Internal Server Error";
                    break;
                case HttpStatusCode.NotImplemented:
                    ReasonPhrase = "Not Implemented";
                    break;
                case HttpStatusCode.BadGateway:
                    ReasonPhrase = "Bad Gateway";
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    ReasonPhrase = "Service Unavailable";
                    break;
                case HttpStatusCode.GatewayTimeout:
                    ReasonPhrase = "Gateway Time-out";
                    break;
                case HttpStatusCode.HttpVersionNotSupported:
                    ReasonPhrase = "HTTP Version not supported";
                    break;
            }
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// HttpStatusCode
    /// https://www.w3.org/Protocols/rfc2616/rfc2616-sec6.html#sec6
    /// </summary>
    public enum HttpStatusCode
    {
        Continue = 100,
        SwitchingProtocols = 101,

        OK = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritativeInformation = 203,
        NoContent = 204,
        ResetContent = 205,
        PartialContent = 206,

        MultipleChoices = 300,
        MovedPermanently = 301,
        Found = 302,
        SeeOther = 303,
        NotModified = 304,
        UseProxy = 305,
        TemporaryRedirect = 307,

        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,

        ProxyAuthenticationRequired = 407,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        RequestEntityTooLarge = 413,
        RequestURITooLarge = 414,
        UnsupportedMediaType = 415,
        RequestedRangeNotSatisfiable = 416,
        ExpectationFailed = 417,

        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HttpVersionNotSupported = 505,
    }
}
