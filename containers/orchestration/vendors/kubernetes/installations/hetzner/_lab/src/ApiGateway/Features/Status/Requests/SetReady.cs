using ApiGateway.Health;
using Common.Web.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Features.Status.Requests;

public static class SetReady
{
    public class Endpoint: IEndpoint
    {
        private const string Path = "/status/ready";

        public void MapEndpoint(WebApplication app)
        {
            app.MapPost(Path, Ok (
                    Body body,
                    [FromServices] ReadinessHealthCheck healthCheck) =>
                {
                    healthCheck.IsReady = body.IsReady;

                    return TypedResults.Ok();
                })
                .AllowAnonymous();
        }
    }

    public record Body(bool IsReady);
}
