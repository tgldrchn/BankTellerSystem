using BankTeller.Core.Models;

namespace BankTeller.Core.Interfaces;

/// <summary>
/// Валютын ханшийн удирдлагын үйлчилгээний гэрээ.
/// Теллерийн апп өөрчилж, Blazor дэлгэц уншина.
/// </summary>
public interface ICurrencyService
{
    /// <summary>
    /// Бүх валютын ханшийг буцаана.
    /// Blazor дэлгэц болон Теллерийн апп ашиглана.
    /// </summary>
    Task<List<CurrencyRate>> GetAllRatesAsync();

    /// <summary>
    /// Тодорхой валютын ханшийг шинэчилнэ.
    /// Зөвхөн Теллер дуудах эрхтэй.
    /// </summary>
    /// <param name="currencyCode">Өөрчлөх валютын код. Жишээ: "USD"</param>
    /// <param name="buyRate">Шинэ худалдан авах ханш</param>
    /// <param name="sellRate">Шинэ худалдах ханш (buyRate-с их байх ёстой)</param>
    Task<bool> UpdateRateAsync(string currencyCode, decimal buyRate, decimal sellRate);
}