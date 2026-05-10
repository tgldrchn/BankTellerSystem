using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;
using BankTeller.Core.Enums;

namespace BankTeller.TellerApp.Services;

/// <summary>
/// <see cref="IQueueService"/>-ийн mock хэрэгжилт.
/// API бэлэн болох хүртэл дотоод санах ойд дугаарын дараалал хадгалж ажиллана.
/// API бэлэн болсон үед Program.cs-д DI-ээр <c>ApiQueueService</c>-ээр солино.
/// </summary>
public class MockQueueService : IQueueService
{
    // ── State ────────────────────────────────────────────────────

    /// <summary>Хамгийн сүүлд олгосон дугаар. Апп дахин эхлэхэд 0-ээс эхэлнэ.</summary>
    private int _current = 0;

    /// <summary>Хүлээж буй үйлчлүүлэгчдийн дараалал.</summary>
    private readonly Queue<QueueTicket> _waiting = new();

    // ── IQueueService ────────────────────────────────────────────

    /// <summary>
    /// Шинэ дугаар олгож дараалалд нэмнэ.
    /// Дугаар 1-ээс эхлэн нэмэгдэнэ.
    /// </summary>
    /// <returns>Шинээр үүссэн <see cref="QueueTicket"/>.</returns>
    public Task<QueueTicket> IssueNextAsync()
    {
        var ticket = new QueueTicket
        {
            Number = ++_current,
            IssuedAt = DateTime.Now,
            Status = TicketStatus.Waiting
        };
        _waiting.Enqueue(ticket);
        return Task.FromResult(ticket);
    }

    /// <summary>
    /// Дараалалд байгаа дараагийн үйлчлүүлэгчийг дуудна.
    /// Дуудагдсан тасалбарын төлөв <see cref="TicketStatus.Called"/> болно.
    /// Дараалал хоосон бол <c>null</c> буцаана.
    /// </summary>
    /// <returns>Дуудагдсан <see cref="QueueTicket"/>, эсвэл <c>null</c>.</returns>
    public Task<QueueTicket?> CallNextAsync()
    {
        if (_waiting.TryDequeue(out var ticket))
        {
            ticket.Status = TicketStatus.Called;
            return Task.FromResult<QueueTicket?>(ticket);
        }
        return Task.FromResult<QueueTicket?>(null);
    }

    /// <summary>
    /// Одоогийн дугаарыг (хамгийн сүүлд олгосон) буцаана.
    /// Дугаар олгогдоогүй бол 0 буцаана.
    /// </summary>
    /// <returns>Сүүлийн дугаар.</returns>
    public Task<int> GetCurrentNumberAsync() =>
        Task.FromResult(_current);

    /// <summary>
    /// Дараалалд хүлээж буй үйлчлүүлэгчдийн тоог буцаана.
    /// MainForm-д "Хүлээж буй: N үйлчлүүлэгч" харуулахад ашиглана.
    /// </summary>
    /// <returns>Хүлээж буй үйлчлүүлэгчдийн тоо.</returns>
    public Task<int> GetWaitingCountAsync() =>
        Task.FromResult(_waiting.Count);
}