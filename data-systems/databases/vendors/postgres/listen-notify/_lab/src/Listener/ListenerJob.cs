using System.Data;
using Npgsql;

namespace Listener;

public class ListenerJob(
    IConfiguration _configuration,
    ILogger<ListenerJob> _logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5_000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("ApplicationDb"));
                await connection.OpenAsync(stoppingToken);

                connection.Notification += OnNotification;

                await using (var cmd = new NpgsqlCommand())
                {
                    cmd.CommandText = "LISTEN cars_channel;";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    await cmd.ExecuteNonQueryAsync(stoppingToken);
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Waiting for notification...");
                    await connection.WaitAsync(stoppingToken);
                    _logger.LogInformation("Notification received");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection error, reconnecting...");
                await Task.Delay(10000, stoppingToken);
            }
        }
    }

    private void OnNotification(object sender, NpgsqlNotificationEventArgs e)
    {
        _logger.LogInformation($"Car created {e.Payload}");
    }
}
