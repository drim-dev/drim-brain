using Listener;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ListenerJob>();

var host = builder.Build();

host.Run();
