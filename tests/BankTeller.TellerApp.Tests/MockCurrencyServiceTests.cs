using BankTeller.TellerApp.Services;
using Xunit;

namespace BankTeller.TellerApp.Tests;

/// <summary>
/// <see cref="MockCurrencyService"/>-ийн unit тестүүд.
/// Валютын ханш унших болон шинэчлэх логикийг шалгана.
/// </summary>
public class MockCurrencyServiceTests
{
    private readonly MockCurrencyService _service = new();

    // ── GetAllRatesAsync ─────────────────────────────────────────

    /// <summary>
    /// Ханш татахад хоосон биш жагсаалт буцаах ёстой.
    /// </summary>
    [Fact]
    public async Task GetAllRatesAsync_ReturnsNonEmptyList()
    {
        var rates = await _service.GetAllRatesAsync();

        Assert.NotEmpty(rates);
    }

    /// <summary>
    /// USD ханш байх ёстой.
    /// </summary>
    [Fact]
    public async Task GetAllRatesAsync_ContainsUSD()
    {
        var rates = await _service.GetAllRatesAsync();

        Assert.Contains(rates, r => r.CurrencyCode == "USD");
    }

    /// <summary>
    /// Бүх ханшийн авах үнэ 0-ээс их байх ёстой.
    /// </summary>
    [Fact]
    public async Task GetAllRatesAsync_AllBuyRatesArePositive()
    {
        var rates = await _service.GetAllRatesAsync();

        Assert.All(rates, r => Assert.True(r.BuyRate > 0));
    }

    /// <summary>
    /// Зарах үнэ авах үнээс их буюу тэнцүү байх ёстой.
    /// </summary>
    [Fact]
    public async Task GetAllRatesAsync_SellRateIsGreaterOrEqualToBuyRate()
    {
        var rates = await _service.GetAllRatesAsync();

        Assert.All(rates, r => Assert.True(r.SellRate >= r.BuyRate));
    }

    // ── UpdateRateAsync ──────────────────────────────────────────

    /// <summary>
    /// Ханш шинэчилсний дараа шинэ утга хадгалагдах ёстой.
    /// </summary>
    [Fact]
    public async Task UpdateRateAsync_ValidRate_UpdatesSuccessfully()
    {
        await _service.UpdateRateAsync("USD", 3500, 3550);
        var rates = await _service.GetAllRatesAsync();
        var usd = rates.First(r => r.CurrencyCode == "USD");

        Assert.Equal(3500, usd.BuyRate);
        Assert.Equal(3550, usd.SellRate);
    }

    /// <summary>
    /// Байхгүй валют шинэчлэхэд алдаа гарахгүй байх ёстой.
    /// </summary>
    [Fact]
    public async Task UpdateRateAsync_InvalidCurrency_DoesNotThrow()
    {
        var exception = await Record.ExceptionAsync(() =>
            _service.UpdateRateAsync("XYZ", 100, 110));

        Assert.Null(exception);
    }
}