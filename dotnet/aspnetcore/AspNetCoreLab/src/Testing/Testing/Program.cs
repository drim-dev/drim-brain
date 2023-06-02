using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Testing.Common.Registration;
using Testing.Database;
using Testing.Errors.Extensions;
using Testing.Features.Auth.Registration;
using Testing.Pipeline.Behaviors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TestingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TestingDbContext")));

builder.Services.AddProblemDetails();

builder.Services.AddMediatR(cfg => cfg
    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
    .AddOpenBehavior(typeof(ValidationBehavior<,>)));

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.AddCommon();

builder.AddAuth();

var app = builder.Build();

app.MapProblemDetails();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
