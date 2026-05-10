using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;

namespace BankTeller.TellerApp.Services;

public class MockCurrencyService : ICurrencyService
{
    private readonly List<CurrencyRate> _rates = new()
    {
        new CurrencyRate { CurrencyCode = "USD", BuyRate = 3420, SellRate = 3450 },
        new CurrencyRate { CurrencyCode = "EUR", BuyRate = 3700, SellRate = 3730 },
        new CurrencyRate { CurrencyCode = "CNY", BuyRate = 470,  SellRate = 480  }
    };

    public Task<List<CurrencyRate>> GetAllRatesAsync() =>
        Task.FromResult(_rates);

    public Task<bool> UpdateRateAsync(string currencyCode, decimal buyRate, decimal sellRate)
    {
        var rate = _rates.FirstOrDefault(r => r.CurrencyCode == currencyCode);
        if (rate == null) return Task.FromResult(false);

        rate.BuyRate = buyRate;
        rate.SellRate = sellRate;
        rate.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(true);
    }
}