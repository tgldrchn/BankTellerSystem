using BankTeller.API.Channels;
using BankTeller.API.Data;
using BankTeller.Core.Enums;
using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTeller.API.Services;

/// <summary>
/// Хүлээлгийн дугаарын үйлчилгээ.
/// NumberDisplayChannel ашиглан нэг дугаарыг олон дэлгэцэнд харуулахаас сэргийлнэ.
/// </summary>
public class QueueService : IQueueService
{
    private readonly AppDbContext _db;
    private readonly NumberDisplayChannel _displayChannel;

    public QueueService(AppDbContext db, NumberDisplayChannel displayChannel)
    {
        _db = db;
        _displayChannel = displayChannel;
    }

    /// <summary>
    /// Шинэ дугаар олгоно.
    /// </summary>
    public async Task<QueueTicket> IssueNextAsync()
    {
        var last = await _db.QueueTickets
            .OrderByDescending(t => t.Number)
            .FirstOrDefaultAsync();

        var ticket = new QueueTicket
        {
            Number = (last?.Number ?? 0) + 1,
            IssuedAt = DateTime.Now,
            Status = TicketStatus.Waiting
        };
        _db.QueueTickets.Add(ticket);
        await _db.SaveChangesAsync();
        return ticket;
    }

    /// <summary>
    /// Дараагийн дугаарыг дуудна.
    /// NumberDisplayChannel-д бичиж Socket сервер дэлгэцэд илгээнэ.
    /// </summary>
    public async Task<QueueTicket?> CallNextAsync()
    {
        var ticket = await _db.QueueTickets
            .Where(t => t.Status == TicketStatus.Waiting)
            .OrderBy(t => t.Number)
            .FirstOrDefaultAsync();

        if (ticket == null) return null;

        ticket.Status = TicketStatus.Called;
        await _db.SaveChangesAsync();

        // Channel-д бичнэ — Socket сервер уншиж дэлгэцэд илгээнэ
        await _displayChannel.WriteAsync(ticket.Number);

        return ticket;
    }

    /// <summary>Одоогийн дуудагдсан дугаарыг буцаана.</summary>
    public async Task<int> GetCurrentNumberAsync()
    {
        var ticket = await _db.QueueTickets
            .Where(t => t.Status == TicketStatus.Called)
            .OrderByDescending(t => t.Number)
            .FirstOrDefaultAsync();
        return ticket?.Number ?? 0;
    }

    /// <summary>Хүлээж буй үйлчлүүлэгчийн тоог буцаана.</summary>
    public async Task<int> GetWaitingCountAsync() =>
        await _db.QueueTickets
            .CountAsync(t => t.Status == TicketStatus.Waiting);
}