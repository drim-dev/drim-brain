using Microsoft.EntityFrameworkCore;
using WebApi.Database.Initialization;

namespace WebApi.Database.Extensions;

public static class DatabaseWebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddDbContext<AppDbContext>(opts => opts.UseNpgsql(builder.Configuration.GetConnectionString("DockerCompose")))
            .AddHostedService<DatabaseMigrationHostedService>();

        return builder;
    }
}
