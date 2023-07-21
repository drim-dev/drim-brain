using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Testing.Tests.Integration.Advanced.Harnesses.Errors.Contracts;

namespace Testing.Tests.Integration.Advanced.Harnesses.Helpers;

public static class ValidationExtensions
{
    public static void ShouldContain(this ValidationProblemDetailsContract contract, string expectedField,
        string expectedMessage, string expectedCode)
    {
        contract.Should().NotBeNull();
        contract.Title.Should().Be("Validation failed");
        contract.Type.Should().Be("https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400");
        contract.Detail.Should().Be("One or more validation errors have occurred");
        contract.Status.Should().Be(StatusCodes.Status400BadRequest);
        contract.Errors.Should().HaveCount(1);

        var error = contract.Errors.Single();
        error.Field.Should().Be(expectedField);
        error.Message.Should().Be(expectedMessage);
        error.Code.Should().Be(expectedCode);
    }
}
