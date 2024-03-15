using FluentValidation;
using MediatR;

namespace Common.Validation;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? _validator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validator is not null)
        {
            var result = await _validator.ValidateAsync(request, cancellationToken);

            if (!result.IsValid)
            {
                throw new ValidationException("Validation failed", result.Errors);
            }
        }

        return await next();
    }
}
