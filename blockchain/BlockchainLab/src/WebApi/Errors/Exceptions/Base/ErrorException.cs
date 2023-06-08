namespace WebApi.Errors.Exceptions.Base;

public class ErrorException : Exception
{
    protected ErrorException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}
