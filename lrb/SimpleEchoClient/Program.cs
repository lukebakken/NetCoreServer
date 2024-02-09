// See https://aka.ms/new-console-template for more information
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;

static string Now()
{
    return DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
}

static void PrintBuffer(PipeReader reader, ReadResult result)
{
    ReadOnlySequence<byte> buffer = result.Buffer;
    if (buffer.IsEmpty && result.IsCompleted)
    {
        Console.Error.WriteLine("[ERROR] {0} unexpected end!", Now());
        Environment.Exit(1);
    }

    foreach (ReadOnlyMemory<byte> segment in buffer)
    {
        byte[] data = segment.ToArray();
        Console.WriteLine("[INFO] {0} msg: {1}", Now(),
            Encoding.ASCII.GetString(data));
    }

    reader.AdvanceTo(buffer.Start, buffer.End);
}

var ep = new IPEndPoint(IPAddress.Loopback, 1111);
var client = new TcpClient(ep);
await client.ConnectAsync(ep);
NetworkStream ns = client.GetStream();

var reader = PipeReader.Create(ns);
var writer = PipeWriter.Create(ns);

byte[] msg = Encoding.ASCII.GetBytes("HELLO!");
await writer.WriteAsync(msg);

ReadResult result = await reader.ReadAsync();
PrintBuffer(reader, result);

using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
{
    try
    {
        Console.WriteLine("[INFO] {0} read with token start...", Now());
        result = await reader.ReadAsync(cts.Token);
        PrintBuffer(reader, result);
        Console.WriteLine("[INFO] {0} read with token end, result: {1}",
            Now(), result);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("[ERROR] {0} ex: {1}", Now(), ex);
    }
}
