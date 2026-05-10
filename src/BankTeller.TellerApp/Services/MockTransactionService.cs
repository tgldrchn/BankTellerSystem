using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;

namespace BankTeller.TellerApp.Services;

public class MockTransactionService : ITransactionService
{
    // Тест данснууд
    private readonly List<Account> _accounts = new()
    {
        new Account { Id = 1, AccountNumber = "ACC001", OwnerName = "Бат",  Balance = 1_000_000, Currency = "MNT" },
        new Account { Id = 2, AccountNumber = "ACC002", OwnerName = "Дорж", Balance = 500_000,   Currency = "MNT" }
    };

    public Task<Account?> GetAccountAsync(string accountNumber)
    {
        var acc = _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        return Task.FromResult(acc);
    }

    public Task<bool> TransferAsync(string fromAccount, string toAccount, decimal amount)
    {
        var from = _accounts.FirstOrDefault(a => a.AccountNumber == fromAccount);
        var to = _accounts.FirstOrDefault(a => a.AccountNumber == toAccount);

        if (from == null || to == null) return Task.FromResult(false);
        if (from.Balance < amount) return Task.FromResult(false);

        from.Balance -= amount;
        to.Balance += amount;
        return Task.FromResult(true);
    }
}