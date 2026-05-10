using System;
using System.Collections.Generic;
using System.Text;
using BankTeller.Core.Models;

namespace BankTeller.NumberTerminal.Services;

public interface IQueueClient
{
    Task<QueueTicket> IssueTicketAsync();
}