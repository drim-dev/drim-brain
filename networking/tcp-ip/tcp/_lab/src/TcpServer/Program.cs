using System.Net;
using System.Net.Sockets;
using System.Text;

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, args) =>
{
    cts.Cancel();
    args.Cancel = true;
};

var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

listener.Bind(new IPEndPoint(IPAddress.Any, 15000));

listener.Listen(10);

Console.WriteLine("Waiting for connections...");

while (!cts.IsCancellationRequested)
{
    try
    {
        var socket = await listener.AcceptAsync(cts.Token);

        Console.WriteLine("Client connected");

        var buffer = new byte[1024];

        var bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None, cts.Token);

        var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        Console.WriteLine($"Received request: {request}");

        if (request.StartsWith("DOWNLOAD "))
        {
            var fileName = request[9..];

            if (string.IsNullOrWhiteSpace(fileName))
            {
                await socket.SendAsync("BAD REQUEST\n"u8.ToArray(), SocketFlags.None, cts.Token);
            }
            else if (!File.Exists($"Files/{fileName}"))
            {
                await socket.SendAsync("NOT FOUND\n"u8.ToArray(), SocketFlags.None, cts.Token);
            }
            else
            {
                var fileInfo = new FileInfo($"Files/{fileName}");

                var sb = new StringBuilder("OK\n\n");
                sb.AppendLine($"LENGTH: {fileInfo.Length}");

                await socket.SendAsync(Encoding.UTF8.GetBytes(sb.ToString()), SocketFlags.None, cts.Token);

                await using var file = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);

                var sendBuffer = new byte[1024];

                while (true)
                {
                    bytesRead = await file.ReadAsync(sendBuffer, cts.Token);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    await socket.SendAsync(sendBuffer.AsMemory()[..bytesRead], SocketFlags.None, cts.Token);
                }
            }
        }
        else if (request.StartsWith("LIST"))
        {
            var sb = new StringBuilder("OK\n\n");
            foreach (var file in Directory.EnumerateFiles("Files"))
            {
                sb.AppendLine(file[6..]);
            }

            var responseBytes = Encoding.UTF8.GetBytes(sb.ToString());
            await socket.SendAsync(responseBytes, SocketFlags.None, cts.Token);
        }
        else if (request.StartsWith("SEARCH "))
        {
            var term = request[7..];

            if (string.IsNullOrWhiteSpace(term))
            {
                await socket.SendAsync("BAD REQUEST\n"u8.ToArray(), SocketFlags.None, cts.Token);
            }
            else
            {
                var sb = new StringBuilder("OK\n\n");
                foreach (var file in Directory.EnumerateFiles("Files").Where(f => f.Contains(term)))
                {
                    sb.AppendLine(file[6..]);
                }

                var responseBytes = Encoding.UTF8.GetBytes(sb.ToString());
                await socket.SendAsync(responseBytes, SocketFlags.None, cts.Token);
            }
        }
        else
        {
            await socket.SendAsync("BAD REQUEST\n"u8.ToArray(), SocketFlags.None, cts.Token);
        }

        socket.Close();
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
