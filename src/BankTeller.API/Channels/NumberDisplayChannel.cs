using System.Threading.Channels;

namespace BankTeller.API.Channels;

/// <summary>
/// Дугаар харуулах Channel сервис.
/// Нэг дугаарыг олон дэлгэцэнд давхар харуулахаас сэргийлнэ.
/// Дугаар бүрийг зөвхөн НЭГ удаа боловсруулна.
/// </summary>
public class NumberDisplayChannel
{
    private readonly Channel<int> _channel =
        Channel.CreateUnbounded<int>();

    public ChannelReader<int> Reader => _channel.Reader;

    /// <summary>
    /// Дуудагдсан дугаарыг Channel-д бичнэ.
    /// Socket сервер уншиж дэлгэцэд илгээнэ.
    /// </summary>
    public async Task WriteAsync(int number) =>
        await _channel.Writer.WriteAsync(number);
}