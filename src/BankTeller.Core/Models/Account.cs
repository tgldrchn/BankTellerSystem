namespace BankTeller.Core.Models;

/// <summary>
/// Банкны харилцагчийн данс.
/// Мөнгө шилжүүлэх гүйлгээнд эх болон очих данс болж ашиглагдана.
/// </summary>
public class Account
{
    /// <summary>Өгөгдлийн сангийн өвөрмөц дугаар</summary>
    public int Id { get; set; }

    /// <summary>
    /// Дансны дугаар. Системд өвөрмөц байна.
    /// Жишээ: "ACC001", "ACC002"
    /// </summary>
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>Дансны эзний нэр</summary>
    public string OwnerName { get; set; } = string.Empty;

    /// <summary>
    /// Дансны үлдэгдэл мөнгөн дүн.
    /// Сөрөг утга авч болохгүй — TransactionService шалгана.
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Дансны валют.
    /// Анхны утга: "MNT" (төгрөг).
    /// Боломжит утга: "MNT", "USD", "EUR", "CNY"
    /// </summary>
    public string Currency { get; set; } = "MNT";
}