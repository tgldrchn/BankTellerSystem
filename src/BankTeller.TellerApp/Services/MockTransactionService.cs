using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;

namespace BankTeller.TellerApp.Services;

/// <summary>
/// API бэлэн болох хүртэл ашиглах мөнгөн гүйлгээний mock үйлчилгээ.
/// SemaphoreSlim ашиглан зэрэг ирсэн давхар гүйлгээнээс сэргийлнэ.
/// </summary>
public class MockTransactionService : ITransactionService
{
    /// <summary>Зэрэг ирсэн гүйлгээг дараалалд оруулах lock</summary>
    private readonly SemaphoreSlim _lock = new(1, 1);

    /// <summary>Санах ойд хадгалагдах тест данснууд</summary>
    private readonly List<Account> _accounts = new()
    {
        new Account { Id = 1, AccountNumber = "ACC001", OwnerName = "Бат",  Balance = 1_000_000, Currency = "MNT" },
        new Account { Id = 2, AccountNumber = "ACC002", OwnerName = "Дорж", Balance = 500_000,   Currency = "MNT" }
    };

    /// <summary>
    /// Дансны мэдээллийг дансны дугаараар хайж буцаана.
    /// </summary>
    /// <param name="accountNumber">Хайх дансны дугаар. Жишээ: "ACC001"</param>
    /// <returns>Олдвол Account, олдохгүй бол null</returns>
    public Task<Account?> GetAccountAsync(string accountNumber)
    {
        var acc = _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        return Task.FromResult(acc);
    }

    /// <summary>
    /// А данснаас Б данс руу мөнгө шилжүүлнэ.
    /// Дараах тохиолдолд false буцаана:
    /// - Дүн 0 буюу сөрөг байвал
    /// - Данс олдохгүй байвал
    /// - Үлдэгдэл хүрэлцэхгүй байвал
    /// </summary>
    /// <param name="fromAccount">Мөнгө гарах дансны дугаар</param>
    /// <param name="toAccount">Мөнгө орох дансны дугаар</param>
    /// <param name="amount">Шилжүүлэх дүн (0-с их байх ёстой)</param>
    /// <returns>Амжилттай бол true, үгүй бол false</returns>
    public async Task<bool> TransferAsync(string fromAccount, string toAccount, decimal amount)
    {
        await _lock.WaitAsync();
        try
        {
            if (amount <= 0) return false;

            var from = _accounts.FirstOrDefault(a => a.AccountNumber == fromAccount);
            var to = _accounts.FirstOrDefault(a => a.AccountNumber == toAccount);

            if (from == null || to == null) return false;
            if (from.Balance < amount) return false;

            from.Balance -= amount;
            to.Balance += amount;
            return true;
        }
        finally { _lock.Release(); }
    }
}