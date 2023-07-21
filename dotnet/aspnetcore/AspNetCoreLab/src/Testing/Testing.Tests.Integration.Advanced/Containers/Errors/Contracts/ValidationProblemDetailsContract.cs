namespace Testing.Tests.Integration.Advanced.Containers.Errors.Contracts;

public class ValidationProblemDetailsContract
{
    public string Title { get; set; }

    public string Type { get; set; }

    public string Detail { get; set; }

    public int Status { get; set; }

    public string TraceId { get; set; }

    public ErrorDataContract[] Errors { get; set; }
}

public class ErrorDataContract
{
    public string Field { get; set; }

    public string Message { get; set; }

    public string Code { get; set; }
}
