using System.Net;
using System.Net.Sockets;
using MessagePack;
using Metrics;

using var client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

var host = Environment.GetEnvironmentVariable("HOST");
var ipAddress = host is null
    ? IPAddress.Loopback
    : Dns.GetHostAddresses(host).First();

while (true)
{
    Console.Write("Enter engine id or 'quit' to exit: ");
    var command = Console.ReadLine();

    if (command == "quit")
    {
        break;
    }

    var engineId = int.Parse(command!);
    Console.Write("Combustion temperature: ");
    var combustionTemperature = decimal.Parse(Console.ReadLine()!);
    Console.Write("Combustion pressure: ");
    var combustionPressure = decimal.Parse(Console.ReadLine()!);
    Console.Write("Oxygen consumption: ");
    var oxygenConsumption = decimal.Parse(Console.ReadLine()!);
    Console.Write("Methane consumption: ");
    var methaneConsumption = decimal.Parse(Console.ReadLine()!);

    var engineMetrics = new EngineMetrics(engineId, combustionTemperature, combustionPressure, oxygenConsumption,
        methaneConsumption);

    await client.SendToAsync(MessagePackSerializer.Serialize(engineMetrics), SocketFlags.None,
        new IPEndPoint(ipAddress, 15000));

    Console.WriteLine("Metrics sent");
}
