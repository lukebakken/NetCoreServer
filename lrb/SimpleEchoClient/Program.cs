// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;

static string Now()
{
    return DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
}

var ep = new IPEndPoint(IPAddress.Loopback, 1111);
var client = new TcpClient(ep);
await client.ConnectAsync(ep);
NetworkStream ns = client.GetStream();


byte[] msg = Encoding.ASCII.GetBytes("HELLO!");
await ns.WriteAsync(msg);

byte[] buf = new byte[1024];
int read = await ns.ReadAsync(buf);

Console.WriteLine("[INFO] {0} read: {1}", Now(), read);
Console.WriteLine("[INFO] {0} msg: {1}", Now(), Encoding.ASCII.GetString(buf));

using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
{
    try
    {
        Console.WriteLine("[INFO] {0} read with token start...", Now());
        read = await ns.ReadAsync(buf, cts.Token);
        Console.WriteLine("[INFO] {0} read with token end", Now());
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("[ERROR] {0} ex: {1}", Now(), ex);
    }
}
