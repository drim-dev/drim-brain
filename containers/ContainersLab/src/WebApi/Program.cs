using WebApi;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var sampleTodos = TodoGenerator.GenerateTodos().ToArray();

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation($"Host name is {Environment.MachineName}");

app.Run();

public partial class Program {}
