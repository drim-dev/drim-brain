using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Protocol commands:\n");

Console.WriteLine("LIST - list all files on the server");
Console.WriteLine("SEARCH <substring> - search for files on the server");
Console.WriteLine("DOWNLOAD <file name> - download file from the server");
Console.WriteLine("quit - exit the program\n");

var host = Environment.GetEnvironmentVariable("HOST");
var ipAddress = host is null
    ? IPAddress.Loopback
    : Dns.GetHostAddresses(host).First();

while (true)
{
    Console.Write("Enter command: ");

    var request = Console.ReadLine();

    if (request == "quit")
    {
        break;
    }

    var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    await client.ConnectAsync(ipAddress, 15000);

    var requestBytes = Encoding.UTF8.GetBytes(request);

    await client.SendAsync(requestBytes, SocketFlags.None);

    var buffer = new byte[1024];

    if (!request.StartsWith("DOWNLOAD "))
    {
        var bytesRead = await client.ReceiveAsync(buffer, SocketFlags.None);

        var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        Console.WriteLine($"\n{response}");
    }
    else
    {
        await client.ReceiveAsync(buffer, SocketFlags.None);
        var response = Encoding.UTF8.GetString(buffer);

        if (response.StartsWith("OK\n\n"))
        {
            var length = long.Parse(response[5..].Split('\n')[0].Split(' ')[1]);

            Directory.CreateDirectory("Downloads");

            var fileName = request[9..];
            await using var file = File.Open($"Downloads/{fileName}", FileMode.Create, FileAccess.Write, FileShare.Write);

            var receivedBytes = 0L;

            while (receivedBytes < length)
            {
                var bytesRead = await client.ReceiveAsync(buffer, SocketFlags.None);

                await file.WriteAsync(buffer.AsMemory(0, bytesRead));

                receivedBytes += bytesRead;
            }

            Console.WriteLine($"File {fileName} downloaded successfully\n");
        }
        else
        {
            Console.WriteLine($"\n{response}");
        }
    }

    client.Close();
}

Console.WriteLine("Goodbye!");
