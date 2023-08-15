using GenericHost.AspNetCore.FastEndpoints;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
    .Build();

await host.RunAsync();
