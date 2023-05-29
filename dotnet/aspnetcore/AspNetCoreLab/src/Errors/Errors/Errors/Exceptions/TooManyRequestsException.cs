using Errors.Errors.Exceptions.Base;

namespace Errors.Errors.Exceptions;

public class TooManyRequestsException : ErrorException
{
    public TooManyRequestsException() : base("Too many requests")
    {
    }
}
