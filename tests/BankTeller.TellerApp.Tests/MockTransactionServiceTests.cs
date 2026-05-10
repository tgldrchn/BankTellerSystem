using BankTeller.TellerApp.Services;
using Xunit;

namespace BankTeller.TellerApp.Tests;

/// <summary>
/// <see cref="MockTransactionService"/>-ийн unit тестүүд.
/// Данс шалгах болон мөнгө шилжүүлэх логикийг шалгана.
/// </summary>
public class MockTransactionServiceTests
{
    private readonly MockTransactionService _service = new();

    // ── GetAccountAsync ──────────────────────────────────────────

    /// <summary>
    /// Байгаа дансны дугаар оруулахад данс буцаах ёстой.
    /// </summary>
    [Fact]
    public async Task GetAccountAsync_ValidAccount_ReturnsAccount()
    {
        var account = await _service.GetAccountAsync("ACC001");

        Assert.NotNull(account);
    }

    /// <summary>
    /// Байгаа дансны дугаар оруулахад зөв дугаар буцаах ёстой.
    /// </summary>
    [Fact]
    public async Task GetAccountAsync_ValidAccount_ReturnsCorrectNumber()
    {
        var account = await _service.GetAccountAsync("ACC001");

        Assert.Equal("ACC001", account!.AccountNumber);
    }

    /// <summary>
    /// Байхгүй дансны дугаар оруулахад null буцаах ёстой.
    /// </summary>
    [Fact]
    public async Task GetAccountAsync_InvalidAccount_ReturnsNull()
    {
        var account = await _service.GetAccountAsync("INVALID");

        Assert.Null(account);
    }

    /// <summary>
    /// Хоосон дансны дугаар оруулахад null буцаах ёстой.
    /// </summary>
    [Fact]
    public async Task GetAccountAsync_EmptyString_ReturnsNull()
    {
        var account = await _service.GetAccountAsync("");

        Assert.Null(account);
    }

    // ── TransferAsync ────────────────────────────────────────────

    /// <summary>
    /// Үлдэгдэл хүрэлцэхэд шилжүүлэг амжилттай болох ёстой.
    /// </summary>
    [Fact]
    public async Task TransferAsync_SufficientBalance_ReturnsTrue()
    {
        var success = await _service.TransferAsync("ACC001", "ACC002", 1000);

        Assert.True(success);
    }

    /// <summary>
    /// Шилжүүлсний дараа гарах дансны үлдэгдэл буурах ёстой.
    /// </summary>
    [Fact]
    public async Task TransferAsync_SufficientBalance_DeductsFromSource()
    {
        var before = (await _service.GetAccountAsync("ACC001"))!.Balance;
        await _service.TransferAsync("ACC001", "ACC002", 1000);
        var after = (await _service.GetAccountAsync("ACC001"))!.Balance;

        Assert.Equal(before - 1000, after);
    }

    /// <summary>
    /// Шилжүүлсний дараа орох дансны үлдэгдэл нэмэгдэх ёстой.
    /// </summary>
    [Fact]
    public async Task TransferAsync_SufficientBalance_AddsToDestination()
    {
        var before = (await _service.GetAccountAsync("ACC002"))!.Balance;
        await _service.TransferAsync("ACC001", "ACC002", 1000);
        var after = (await _service.GetAccountAsync("ACC002"))!.Balance;

        Assert.Equal(before + 1000, after);
    }

    /// <summary>
    /// Үлдэгдэл хүрэлцэхгүй үед шилжүүлэг амжилтгүй болох ёстой.
    /// </summary>
    [Fact]
    public async Task TransferAsync_InsufficientBalance_ReturnsFalse()
    {
        var success = await _service.TransferAsync("ACC001", "ACC002", 999_999_999);

        Assert.False(success);
    }

    /// <summary>
    /// Байхгүй данс оруулахад амжилтгүй болох ёстой.
    /// </summary>
    [Fact]
    public async Task TransferAsync_InvalidAccount_ReturnsFalse()
    {
        var success = await _service.TransferAsync("INVALID", "ACC002", 1000);

        Assert.False(success);
    }

    /// <summary>
    /// Сөрөг дүн оруулахад амжилтгүй болох ёстой.
    /// </summary>
    [Fact]
    public async Task TransferAsync_NegativeAmount_ReturnsFalse()
    {
        var success = await _service.TransferAsync("ACC001", "ACC002", -1000);

        Assert.False(success);
    }
}