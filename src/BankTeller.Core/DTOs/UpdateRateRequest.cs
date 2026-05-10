namespace BankTeller.Core.DTOs;

/// <summary>Валютын ханш шинэчлэх хүсэлтийн өгөгдөл</summary>
public class UpdateRateRequest
{
    /// <summary>Өөрчлөх валютын код. Жишээ: "USD"</summary>
    public string CurrencyCode { get; set; } = string.Empty;

    /// <summary>Шинэ авах ханш</summary>
    public decimal BuyRate { get; set; }

    /// <summary>Шинэ зарах ханш (BuyRate-с их байх ёстой)</summary>
    public decimal SellRate { get; set; }
}