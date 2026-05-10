using BankTeller.API.Channels;
using BankTeller.API.Data;
using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTeller.API.Services;

/// <summary>
/// Мөнгөн гүйлгээний үйлчилгээ.
/// TransactionChannel-аар дамжуулж давхар гүйлгээнээс сэргийлнэ.
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly AppDbContext _db;
    private readonly TransactionChannel _channel;

    public TransactionService(AppDbContext db, TransactionChannel channel)
    {
        _db = db;
        _channel = channel;
    }

    /// <summary>Дансны мэдээллийг дугаараар хайна.</summary>
    public async Task<Account?> GetAccountAsync(string accountNumber) =>
        await _db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

    /// <summary>
    /// Гүйлгээг Channel-д дамжуулна.
    /// Channel нэг нэгээр боловсруулж давхар гүйлгээнээс сэргийлнэ.
    /// </summary>
    public async Task<bool> TransferAsync(string fromAccount, string toAccount, decimal amount) =>
        await _channel.EnqueueAsync(fromAccount, toAccount, amount);
}