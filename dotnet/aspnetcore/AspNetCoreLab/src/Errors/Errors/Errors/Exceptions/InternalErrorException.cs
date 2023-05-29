using Errors.Errors.Exceptions.Base;

namespace Errors.Errors.Exceptions;

public class InternalErrorException : ErrorException
{
    public InternalErrorException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}
