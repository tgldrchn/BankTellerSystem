using BankTeller.API.Channels;
using BankTeller.API.Data;
using BankTeller.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankTeller.API.Tests;

/// <summary>
/// TransactionChannel-ийн unit тестүүд.
/// Давхар гүйлгээнээс сэргийлж байгааг шалгана.
/// </summary>
public class TransactionChannelTests : IAsyncLifetime
{
    private TransactionChannel _channel = null!;
    private ServiceProvider _provider = null!;
    private AppDbContext _db = null!;

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseInMemoryDatabase(Guid.NewGuid().ToString()),
            ServiceLifetime.Singleton);

        services.AddSingleton<TransactionChannel>();
        services.AddHostedService(p => p.GetRequiredService<TransactionChannel>());

        _provider = services.BuildServiceProvider();
        _channel = _provider.GetRequiredService<TransactionChannel>();
        _db = _provider.GetRequiredService<AppDbContext>();

        // Seed өгөгдөл
        _db.Accounts.AddRange(
            new Account { Id = 1, AccountNumber = "ACN001", OwnerName = "Төгөлдөр", Balance = 3_500_000, Currency = "MNT" },
            new Account { Id = 2, AccountNumber = "ACN002", OwnerName = "Сарантуяа", Balance = 1_250_000, Currency = "MNT" },
            new Account { Id = 3, AccountNumber = "ACN003", OwnerName = "Энхжаргал", Balance = 2_800_000, Currency = "MNT" }
        );
        await _db.SaveChangesAsync();

        await _channel.StartAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _channel.StopAsync(CancellationToken.None);
        await _provider.DisposeAsync();
    }

    // ── Энгийн гүйлгээ ───────────────────────────────────────────

    /// <summary>Энгийн гүйлгээ амжилттай болох ёстой</summary>
    [Fact]
    public async Task Transfer_ValidAccounts_ReturnsTrue()
    {
        var result = await _channel.EnqueueAsync("ACN001", "ACN002", 100_000);

        Assert.True(result);
    }

    /// <summary>Данс олдохгүй бол false буцаах ёстой</summary>
    [Fact]
    public async Task Transfer_InvalidAccount_ReturnsFalse()
    {
        var result = await _channel.EnqueueAsync("ACN999", "ACN002", 100_000);

        Assert.False(result);
    }

    /// <summary>Үлдэгдэл хүрэлцэхгүй бол false буцаах ёстой</summary>
    [Fact]
    public async Task Transfer_InsufficientBalance_ReturnsFalse()
    {
        var result = await _channel.EnqueueAsync("ACN001", "ACN002", 99_999_999);

        Assert.False(result);
    }

    /// <summary>Сөрөг дүн бол false буцаах ёстой</summary>
    [Fact]
    public async Task Transfer_NegativeAmount_ReturnsFalse()
    {
        var result = await _channel.EnqueueAsync("ACN001", "ACN002", -1000);

        Assert.False(result);
    }

    /// <summary>0 дүн бол false буцаах ёстой</summary>
    [Fact]
    public async Task Transfer_ZeroAmount_ReturnsFalse()
    {
        var result = await _channel.EnqueueAsync("ACN001", "ACN002", 0);

        Assert.False(result);
    }

    /// <summary>Гүйлгээний дараа үлдэгдэл зөв өөрчлөгдөх ёстой</summary>
    [Fact]
    public async Task Transfer_Success_BalanceUpdatedCorrectly()
    {
        await _channel.EnqueueAsync("ACN001", "ACN002", 500_000);

        var from = await _db.Accounts.FindAsync(1);
        var to = await _db.Accounts.FindAsync(2);

        Assert.Equal(3_000_000, from!.Balance);
        Assert.Equal(1_750_000, to!.Balance);
    }

    /// <summary>Өөр данс руу шилжүүлэхэд нөлөөлөхгүй байх ёстой</summary>
    [Fact]
    public async Task Transfer_Success_OtherAccountUnchanged()
    {
        await _channel.EnqueueAsync("ACN001", "ACN002", 500_000);

        var other = await _db.Accounts.FindAsync(3);

        Assert.Equal(2_800_000, other!.Balance);
    }

    // ── Давхар гүйлгээ ───────────────────────────────────────────

    /// <summary>
    /// Зэрэг ирсэн 2 гүйлгээнд зөвхөн нэг нь амжилттай болох ёстой.
    /// </summary>
    [Fact]
    public async Task Transfer_Concurrent_OnlyOneSucceeds()
    {
        var task1 = _channel.EnqueueAsync("ACN001", "ACN002", 2_000_000);
        var task2 = _channel.EnqueueAsync("ACN001", "ACN003", 2_000_000);

        var results = await Task.WhenAll(task1, task2);

        Assert.Equal(1, results.Count(r => r == true));
        Assert.Equal(1, results.Count(r => r == false));
    }

    /// <summary>
    /// Зэрэг гүйлгээний дараа үлдэгдэл зөв байх ёстой.
    /// </summary>
    [Fact]
    public async Task Transfer_Concurrent_BalanceCorrectAfter()
    {
        var task1 = _channel.EnqueueAsync("ACN001", "ACN002", 2_000_000);
        var task2 = _channel.EnqueueAsync("ACN001", "ACN003", 2_000_000);

        await Task.WhenAll(task1, task2);

        var from = await _db.Accounts.FindAsync(1);

        // Зөвхөн нэг гүйлгээ хийгдсэн тул 3,500,000 - 2,000,000 = 1,500,000
        Assert.Equal(1_500_000, from!.Balance);
    }

    /// <summary>
    /// Олон зэрэг гүйлгээнд үлдэгдэл хэзээ ч сөрөг болохгүй.
    /// </summary>
    [Fact]
    public async Task Transfer_ManyConcurrent_BalanceNeverNegative()
    {
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => _channel.EnqueueAsync("ACN001", "ACN002", 500_000));

        await Task.WhenAll(tasks);

        var from = await _db.Accounts.FindAsync(1);

        Assert.True(from!.Balance >= 0);
    }
}