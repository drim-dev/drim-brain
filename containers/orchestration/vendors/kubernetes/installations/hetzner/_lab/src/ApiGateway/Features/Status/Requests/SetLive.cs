using ApiGateway.Health;
using Common.Web.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Features.Status.Requests;

public static class SetLive
{
    public class Endpoint: IEndpoint
    {
        private const string Path = "/status/live";

        public void MapEndpoint(WebApplication app)
        {
            app.MapPost(Path, Ok (
                    Body body,
                    [FromServices] LivenessHealthCheck healthCheck) =>
                {
                    healthCheck.IsAlive = body.IsLive;

                    return TypedResults.Ok();
                })
                .AllowAnonymous();
        }
    }

    public record Body(bool IsLive);
}
