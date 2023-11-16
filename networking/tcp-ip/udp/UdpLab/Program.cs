using System.CommandLine;
using System.Net;
using System.Net.Sockets;
using System.Text;

const int PacketSize = 1380;

var rootCommand = new RootCommand("UdpLab");

var serverCommand = new Command("server", "Server mode");
rootCommand.AddCommand(serverCommand);

serverCommand.SetHandler(async () =>
{
    byte[] buffer = GC.AllocateArray<byte>(length: 65527, pinned: true);
    Memory<byte> bufferMem = buffer.AsMemory();

    using var udpSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);
    udpSocket.Bind(new IPEndPoint(IPAddress.Any, 12345));

    var result = await udpSocket.ReceiveFromAsync(bufferMem, SocketFlags.None, new IPEndPoint(IPAddress.Any, 0));

    var message = Encoding.UTF8.GetString(bufferMem.Span[..result.ReceivedBytes]);

    Console.WriteLine($"Received packet from {result.RemoteEndPoint} with message: {message}");
});

var clientCommand = new Command("client", "Client mode");
rootCommand.AddCommand(clientCommand);

clientCommand.SetHandler(async () =>
{
    using var udpSocket = new Socket(SocketType.Dgram, ProtocolType.Udp);

    var destination = new IPEndPoint(IPAddress.Loopback, 12345);

    var bytes = "Hello world"u8.ToArray();

    await udpSocket.SendToAsync(bytes, SocketFlags.None, destination);

    Console.WriteLine("Sent packet");
});

await rootCommand.InvokeAsync(args);
