using BankTeller.API.Data;
using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTeller.API.Services;

/// <summary>
/// Валютын ханшийн үйлчилгээ.
/// Теллер ханш өөрчилж, Blazor дэлгэц real-time авна.
/// </summary>
public class CurrencyService : ICurrencyService
{
    private readonly AppDbContext _db;
    public CurrencyService(AppDbContext db) => _db = db;

    /// <summary>Бүх валютын ханшийг буцаана.</summary>
    public async Task<List<CurrencyRate>> GetAllRatesAsync() =>
        await _db.CurrencyRates.ToListAsync();

    /// <summary>
    /// Валютын ханшийг шинэчилнэ.
    /// Зарах ханш авах ханшаас бага бол false буцаана.
    /// </summary>
    public async Task<bool> UpdateRateAsync(string currencyCode, decimal buyRate, decimal sellRate)
    {
        var rate = await _db.CurrencyRates
            .FirstOrDefaultAsync(r => r.CurrencyCode == currencyCode);

        if (rate == null) return false;
        if (sellRate <= buyRate) return false;

        rate.BuyRate = buyRate;
        rate.SellRate = sellRate;
        rate.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}