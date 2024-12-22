using System.Collections.ObjectModel;
using System.Net;

namespace AppTemp.Core.Exceptions;
public class UnauthorizedException : AppException
{
    public UnauthorizedException()
        : base("authentication failed", new Collection<string>(), HttpStatusCode.Unauthorized)
    {
    }
    public UnauthorizedException(string message)
       : base(message, new Collection<string>(), HttpStatusCode.Unauthorized)
    {
    }
}
