namespace BankTeller.Core.Models;

/// <summary>
/// Валютын ханшийн мэдээлэл.
/// Теллер ханшийг өөрчилж, Blazor дэлгэц real-time харуулна.
/// </summary>
public class CurrencyRate
{
    /// <summary>Өгөгдлийн сангийн өвөрмөц дугаар</summary>
    public int Id { get; set; }

    /// <summary>
    /// Валютын код (ISO 4217 стандарт).
    /// Жишээ: "USD", "EUR", "CNY"
    /// </summary>
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// Худалдан авах ханш (банк валют худалдаж авах үнэ).
    /// Жишээ: 1 USD = 3420 MNT
    /// </summary>
    public decimal BuyRate { get; set; }

    /// <summary>
    /// Худалдах ханш (банк валют зарах үнэ).
    /// Жишээ: 1 USD = 3450 MNT. BuyRate-с их байна.
    /// </summary>
    public decimal SellRate { get; set; }

    /// <summary>Ханш хамгийн сүүлд өөрчлөгдсөн цаг (UTC)</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}