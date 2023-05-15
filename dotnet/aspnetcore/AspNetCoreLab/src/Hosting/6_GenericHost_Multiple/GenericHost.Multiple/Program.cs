using GenericHost.Multiple;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host1 = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<HostedService1>();
    })
    .Build();

var host2 = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<HostedService2>();
    })
    .Build();

await Task.WhenAll(
    host1.RunAsync(),
    host2.RunAsync());
