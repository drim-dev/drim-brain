using Testing.Errors.Exceptions.Base;

namespace Testing.Errors.Exceptions;

public class LogicConflictException : ErrorException
{
    public LogicConflictException(string message, string code) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
