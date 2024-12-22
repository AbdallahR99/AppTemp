using System.Net;

namespace AppTemp.Core.Exceptions;
public class AppException : Exception
{
    public IEnumerable<string> ErrorMessages { get; }

    public HttpStatusCode StatusCode { get; }

    public AppException(string message, IEnumerable<string> errors, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }

    public AppException(string message) : base(message)
    {
        ErrorMessages = new List<string>();
    }
}
