using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Enter a request to send to the server, or 'quit' to exit");

while (true)
{
    var request = Console.ReadLine();

    if (request == "quit")
    {
        break;
    }

    var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    await client.ConnectAsync(IPAddress.Loopback, 15000);

    var requestBytes = Encoding.UTF8.GetBytes(request);

    await client.SendAsync(requestBytes, SocketFlags.None);

    var buffer = new byte[1024];

    if (request.StartsWith("DOWNLOAD "))
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
    else
    {
        var bytesRead = await client.ReceiveAsync(buffer, SocketFlags.None);

        var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        Console.WriteLine($"\n{response}");
    }

    client.Close();
}

Console.WriteLine("Goodbye!");
