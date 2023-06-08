using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Database;
using WebApi.Errors.Extensions;
using WebApi.Pipeline.Behaviors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<BlockchainDbContext>(opts => opts.UseNpgsql("Host=localhost;Database=Blockchain;Username=db_creator;Password=12345678;Maximum Pool Size=50;Connection Idle Lifetime=60;"));

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

var app = builder.Build();

app.MapProblemDetails();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
