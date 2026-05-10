using BankTeller.API.Data;
using BankTeller.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace BankTeller.API.Channels;

/// <summary>
/// Гүйлгээний Channel сервис.
/// Олон хүсэлт зэрэг ирэхэд дараалалд оруулж нэг нэгээр боловсруулна.
/// Давхар гүйлгээ хийхээс сэргийлнэ.
/// </summary>
public class TransactionChannel : BackgroundService
{
    private readonly Channel<TransactionRequest> _channel =
        Channel.CreateUnbounded<TransactionRequest>();

    private readonly IServiceScopeFactory _scopeFactory;

    public TransactionChannel(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// Гүйлгээний хүсэлтийг дараалалд нэмнэ.
    /// Хариуг TaskCompletionSource-оор хүлээнэ.
    /// </summary>
    public async Task<bool> EnqueueAsync(string fromAccount, string toAccount, decimal amount)
    {
        var tcs = new TaskCompletionSource<bool>();
        await _channel.Writer.WriteAsync(new TransactionRequest
        {
            FromAccount = fromAccount,
            ToAccount = toAccount,
            Amount = amount,
            Result = tcs
        });
        return await tcs.Task;
    }

    /// <summary>
    /// Background-д ажиллаж Channel-с уншиж гүйлгээг боловсруулна.
    /// Нэг дор зөвхөн НЭГ гүйлгээ хийгдэнэ.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var success = await ProcessAsync(db, request);
            request.Result.SetResult(success);
        }
    }

    private static async Task<bool> ProcessAsync(AppDbContext db, TransactionRequest request)
    {
        if (request.Amount <= 0) return false;

        var from = await db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == request.FromAccount);
        var to = await db.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == request.ToAccount);

        if (from == null || to == null) return false;
        if (from.Balance < request.Amount) return false;

        from.Balance -= request.Amount;
        to.Balance += request.Amount;

        db.Transactions.Add(new Transaction
        {
            FromAccount = request.FromAccount,
            ToAccount = request.ToAccount,
            Amount = request.Amount,
            CreatedAt = DateTime.UtcNow,
            IsSuccess = true
        });

        await db.SaveChangesAsync();
        return true;
    }
}

/// <summary>Channel-д дамжуулах гүйлгээний хүсэлт</summary>
public class TransactionRequest
{
    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TaskCompletionSource<bool> Result { get; set; } = new();
}