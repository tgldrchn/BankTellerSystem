using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;

namespace BankTeller.TellerApp.Services;

/// <summary>
/// API бэлэн болох хүртэл ашиглах валютын ханшийн mock үйлчилгээ.
/// Өгөгдлийг санах ойд хадгална.
/// </summary>
public class MockCurrencyService : ICurrencyService
{
    private readonly List<CurrencyRate> _rates = new()
    {
        new CurrencyRate { CurrencyCode = "USD", BuyRate = 3420, SellRate = 3450, UpdatedAt = DateTime.UtcNow },
        new CurrencyRate { CurrencyCode = "EUR", BuyRate = 3700, SellRate = 3730, UpdatedAt = DateTime.UtcNow },
        new CurrencyRate { CurrencyCode = "CNY", BuyRate = 470,  SellRate = 480,  UpdatedAt = DateTime.UtcNow }
    };

    /// <summary>
    /// Бүх валютын ханшийг буцаана.
    /// </summary>
    public Task<List<CurrencyRate>> GetAllRatesAsync() =>
        Task.FromResult(_rates);

    /// <summary>
    /// Тодорхой валютын ханшийг шинэчилнэ.
    /// Валют олдохгүй бол false буцаана.
    /// Зарах ханш авах ханшаас бага бол false буцаана.
    /// </summary>
    /// <param name="currencyCode">Өөрчлөх валютын код. Жишээ: "USD"</param>
    /// <param name="buyRate">Шинэ авах ханш</param>
    /// <param name="sellRate">Шинэ зарах ханш (buyRate-с их байх ёстой)</param>
    public Task<bool> UpdateRateAsync(string currencyCode, decimal buyRate, decimal sellRate)
    {
        var rate = _rates.FirstOrDefault(r => r.CurrencyCode == currencyCode);
        if (rate == null) return Task.FromResult(false);
        if (sellRate <= buyRate) return Task.FromResult(false);

        rate.BuyRate = buyRate;
        rate.SellRate = sellRate;
        rate.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(true);
    }
}