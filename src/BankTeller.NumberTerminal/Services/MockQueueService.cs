using System;
using System.Collections.Generic;
using System.Text;

using BankTeller.Core.Enums;
using BankTeller.Core.Models;

namespace BankTeller.NumberTerminal.Services;

public class MockQueueService : IQueueClient
{
    private int _currentNumber = 0;

    public Task<QueueTicket> IssueTicketAsync()
    {
        _currentNumber++;

        var ticket = new QueueTicket
        {
            Id = _currentNumber,
            Number = _currentNumber,
            IssuedAt = DateTime.UtcNow,
            Status = TicketStatus.Waiting
        };

        return Task.FromResult(ticket);
    }
}