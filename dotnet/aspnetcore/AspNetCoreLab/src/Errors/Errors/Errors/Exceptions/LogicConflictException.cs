﻿using Errors.Errors.Exceptions.Base;

namespace Errors.Errors.Exceptions;

public class LogicConflictException : ErrorException
{
    public LogicConflictException(string message, string code) : base(message)
    {
        Code = code;
    }

    public string Code { get; }
}
