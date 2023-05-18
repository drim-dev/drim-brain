using System.Text;
using System.Text.Json;
using GenericHost.Kestrel.Endpoints.Controllers.Extensions;
using GenericHost.Kestrel.Endpoints.Dtos;
using GenericHost.Kestrel.Endpoints.Endpoints;
using GenericHost.Kestrel.Endpoints.HostedServices;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Services.Configuration;

namespace GenericHost.Kestrel.Endpoints.Controllers;

public class DepositsController : Controller
{
    [RateLimiting(10_000)]
    [Path("/deposits")]
    public async Task Get(HttpApplicationContext context, IServiceScope scope)
    {
        var depositRepository = scope.ServiceProvider.GetRequiredService<IDepositRepository>();

        var depositModels = (await depositRepository.LoadAllDeposits(CancellationToken.None))
            .Select(x => new DepositDto
            {
                UserId = x.UserId,
                Currency = x.Currency,
                Amount = x.Amount,
                IsConfirmed = x.IsConfirmed,
            });

        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        responseFeature.Headers.Add("Content-Type", new StringValues("application/json; charset=UTF-8"));
        await responseBodyFeature.Stream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(depositModels)));
    }
}
