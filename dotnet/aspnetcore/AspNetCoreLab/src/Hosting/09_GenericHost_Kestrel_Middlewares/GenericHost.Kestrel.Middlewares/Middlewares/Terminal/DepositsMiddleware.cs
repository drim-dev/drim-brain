using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Services.Configuration;

namespace GenericHost.Kestrel.Middlewares.Middlewares.Terminal;

public class DepositsMiddleware : IPipelineMiddleware
{
    private static DateTime _previousDepositsRequestTime = DateTime.MinValue;

    public async Task Invoke(HttpApplicationContext context, IServiceScope scope, Func<Task> next)
    {
        var requestFeature = context.Features.Get<IHttpRequestFeature>()!;
        var responseFeature = context.Features.Get<IHttpResponseFeature>()!;
        var responseBodyFeature = context.Features.Get<IHttpResponseBodyFeature>()!;

        if (!requestFeature.Path.Equals("/deposits", StringComparison.InvariantCultureIgnoreCase))
        {
            await next();
            return;
        }

        if (DateTime.UtcNow - _previousDepositsRequestTime < TimeSpan.FromSeconds(10))
        {
            responseFeature.StatusCode = StatusCodes.Status429TooManyRequests;
        }
        else
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

            responseFeature.Headers.Add("Content-Type", new StringValues("application/json; charset=UTF-8"));
            await responseBodyFeature.Stream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(depositModels)));

            _previousDepositsRequestTime = DateTime.UtcNow;
        }
    }
}
