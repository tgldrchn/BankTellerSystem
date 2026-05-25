using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Text;

namespace BankTeller.NumberDisplay.Services;

/// <summary>
/// Socket клиент.
/// API-ийн Socket серверт холбогдож дугаар хүлээн авна.
/// appsettings.json-с холболтын хаяг, портыг уншина.
/// </summary>
public class SocketClient : IDisposable
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly string _host;
    private readonly int _port;

    /// <summary>Шинэ дугаар ирэхэд энэ event дуудагдана</summary>
    public event Action<int>? NumberReceived;

    /// <summary>Холболт тасарвал энэ event дуудагдана</summary>
    public event Action? Disconnected;

    public SocketClient()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        _host = config["SocketHost"] ?? "localhost";
        _port = int.TryParse(config["SocketPort"], out var port) ? port : 5201;
    }

    /// <summary>
    /// Socket серверт холбогдож дугаар хүлээн авч эхэлнэ.
    /// Холболт тасарвал 3 секунд хүлээгээд дахин холбогдоно.
    /// </summary>
    public async Task ConnectAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(_host, _port, ct);
                _stream = _client.GetStream();
                await ReceiveAsync(ct);
            }
            catch (OperationCanceledException) { break; }
            catch
            {
                Disconnected?.Invoke();
                await Task.Delay(3000, ct);
            }
        }
    }

    /// <summary>
    /// Серверээс дугаар хүлээн авна.
    /// \n тэмдэгээр тусгаарлагдсан дугааруудыг боловсруулна.
    /// </summary>
    private async Task ReceiveAsync(CancellationToken ct)
    {
        var buffer = new byte[1024];
        var sb = new StringBuilder();

        while (!ct.IsCancellationRequested)
        {
            var read = await _stream!.ReadAsync(buffer, ct);
            if (read == 0) break;

            sb.Append(Encoding.UTF8.GetString(buffer, 0, read));

            var lines = sb.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (int.TryParse(line.Trim(), out var number))
                    NumberReceived?.Invoke(number);
            }
            sb.Clear();
        }
    }

    public void Dispose()
    {
        _stream?.Dispose();
        _client?.Dispose();
    }
}