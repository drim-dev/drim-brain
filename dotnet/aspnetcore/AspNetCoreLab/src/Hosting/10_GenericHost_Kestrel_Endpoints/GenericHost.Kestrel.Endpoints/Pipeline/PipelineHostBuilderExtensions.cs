namespace GenericHost.Kestrel.Endpoints.Pipeline;

public static class PipelineHostBuilderExtensions
{
    public static IHostBuilder AddPipeline(this IHostBuilder hostBuilder, Action<IPipelineBuilder> configurePipelineBuilder)
    {
        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            var pipelineBuilder = new PipelineBuilder(services);
            configurePipelineBuilder(pipelineBuilder);

            var pipeline = pipelineBuilder.Build();

            services.AddSingleton(pipeline);
        });

        return hostBuilder;
    }
}
