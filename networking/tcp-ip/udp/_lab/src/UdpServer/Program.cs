using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using MessagePack;
using Metrics;
using UdpServer;

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, args) =>
{
    cts.Cancel();
    args.Cancel = true;
};

var metricsStorage = new MetricsStorage();

using var socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
socket.Bind(new IPEndPoint(IPAddress.Any, 15000));

var buffer = new byte[65526];

while (!cts.IsCancellationRequested)
{
    try
    {
        var result = await socket.ReceiveFromAsync(buffer, SocketFlags.None, new IPEndPoint(IPAddress.Any, 0), cts.Token);

        var engineMetrics = MessagePackSerializer.Deserialize<EngineMetrics>(buffer.AsMemory()[..result.ReceivedBytes]);
        metricsStorage.Save(engineMetrics);

        Console.WriteLine($"Saved engine metrics {JsonSerializer.Serialize(engineMetrics)}");
    }
    catch (OperationCanceledException)
    {
        break;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex}");
    }
}

Console.WriteLine("Goodbye!");
