using BankTeller.API.Channels;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BankTeller.API.Sockets;

/// <summary>
/// TCP Socket сервер.
/// Дугаар харуулах дэлгэцүүдтэй холбогдож,
/// NumberDisplayChannel-с дугаар авч дэлгэцүүдэд илгээнэ.
/// Нэг дугаарыг олон дэлгэцэнд давхар харуулахаас сэргийлнэ.
/// </summary>
public class SocketServer : BackgroundService
{
    private readonly NumberDisplayChannel _channel;
    private readonly ILogger<SocketServer> _logger;

    /// <summary>Холбогдсон клиентүүдийн жагсаалт</summary>
    private readonly ConcurrentDictionary<Guid, TcpClient> _clients = new();

    private TcpListener _listener = null!;
    private const int Port = 5201;

    public SocketServer(NumberDisplayChannel channel, ILogger<SocketServer> logger)
    {
        _channel = channel;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _listener = new TcpListener(IPAddress.Any, Port);
        _listener.Start();
        _logger.LogInformation("Socket сервер port {Port} дээр эхэллээ", Port);

        await Task.WhenAll(
            AcceptClientsAsync(stoppingToken),
            BroadcastNumbersAsync(stoppingToken)
        );
    }

    /// <summary>
    /// Шинэ клиент холбогдохыг хүлээж тасралтгүй ажиллана.
    /// Клиент бүрийг тусдаа Task-д удирдана.
    /// </summary>
    private async Task AcceptClientsAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync(ct);
                var id = Guid.NewGuid();
                _clients.TryAdd(id, client);
                _logger.LogInformation("Шинэ дэлгэц холбогдлоо. Нийт: {Count}", _clients.Count);

                // Клиент тасарвал жагсаалтаас хасах
                _ = MonitorClientAsync(id, client, ct);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Клиент хүлээхэд алдаа гарлаа");
            }
        }
    }

    /// <summary>
    /// Клиент тасарсан эсэхийг хянана.
    /// Тасарвал жагсаалтаас хасна.
    /// </summary>
    private async Task MonitorClientAsync(Guid id, TcpClient client, CancellationToken ct)
    {
        try
        {
            var buffer = new byte[1];
            var stream = client.GetStream();

            // Клиент тасарах хүртэл хүлээнэ
            while (!ct.IsCancellationRequested)
            {
                var read = await stream.ReadAsync(buffer, ct);
                if (read == 0) break;
            }
        }
        catch { }
        finally
        {
            _clients.TryRemove(id, out _);
            client.Dispose();
            _logger.LogInformation("Дэлгэц тасарлаа. Нийт: {Count}", _clients.Count);
        }
    }

    /// <summary>
    /// NumberDisplayChannel-с дугаар уншиж
    /// бүх холбогдсон дэлгэцэд broadcast хийнэ.
    /// Channel нэг л Reader-тай тул нэг дугаарыг
    /// зөвхөн НЭГ удаа уншиж broadcast хийнэ —
    /// олон дэлгэцэнд давхар харуулахаас сэргийлнэ.
    /// </summary>
    private async Task BroadcastNumbersAsync(CancellationToken ct)
    {
        await foreach (var number in _channel.Reader.ReadAllAsync(ct))
        {
            var message = Encoding.UTF8.GetBytes($"{number}\n");
            var dead = new List<Guid>();

            foreach (var (id, client) in _clients)
            {
                try
                {
                    await client.GetStream().WriteAsync(message, ct);
                    _logger.LogInformation("Дугаар {Number} дэлгэцэд илгээлээ", number);
                }
                catch
                {
                    dead.Add(id);
                }
            }

            // Тасарсан клиентүүдийг хасна
            foreach (var id in dead)
            {
                _clients.TryRemove(id, out var c);
                c?.Dispose();
            }
        }
    }

    /// <summary>
    /// Сервер зогсоход бүх клиент холболтыг таслана.
    /// </summary>
    public override async Task StopAsync(CancellationToken ct)
    {
        _listener?.Stop();
        foreach (var (_, client) in _clients)
            client.Dispose();
        _clients.Clear();
        await base.StopAsync(ct);
    }
}