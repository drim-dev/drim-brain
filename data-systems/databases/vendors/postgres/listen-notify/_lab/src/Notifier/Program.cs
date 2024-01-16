using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/notify", async Task<Ok> ([FromBody] CarModel car, CancellationToken cancellationToken) =>
{
    await using var connection = new NpgsqlConnection(builder.Configuration.GetConnectionString("ApplicationDb"));
    await connection.OpenAsync(cancellationToken);

    await using (var cmd = new NpgsqlCommand())
    {
        cmd.CommandText = $"INSERT INTO \"cars\" (name, color, year, mileage) VALUES ('{car.Name}', '{car.Color}', {car.Year}, {car.Mileage});";
        cmd.CommandType = CommandType.Text;
        cmd.Connection = connection;
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    await using (var cmd = new NpgsqlCommand())
    {
        cmd.CommandText = $"NOTIFY cars_channel, '{JsonSerializer.Serialize(car)}';";
        cmd.CommandType = CommandType.Text;
        cmd.Connection = connection;
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    return TypedResults.Ok();
});

app.Run();

public class CarModel
{
    public string Name { get; set; }
    public string Color { get; set; }
    public int Year { get; set; }
    public int Mileage { get; set; }
}
