using LoanService.Api;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiGateway;

public static class Endpoints
{
    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapGet("/loans/offerings",  async Task<Ok<LoanOfferingDto[]>> (LoanServiceClient client, CancellationToken cancellationToken) =>
        {
            var offerings = await client.GetOfferings(cancellationToken);
            return TypedResults.Ok(offerings);
        });

        return app;
    }
}
