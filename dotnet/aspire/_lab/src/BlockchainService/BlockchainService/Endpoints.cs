namespace BlockchainService;

public static class Endpoints
{
    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => "Hello World!");

        return app;
    }
}
