namespace BankTeller.Core.Models;

/// <summary>
/// Теллерийн гүйцэтгэсэн мөнгөн гүйлгээний бүртгэл.
/// А данснаас Б данс руу мөнгө шилжүүлэхэд үүсдэг.
/// </summary>
public class Transaction
{
    /// <summary>Өгөгдлийн сангийн өвөрмөц дугаар</summary>
    public int Id { get; set; }

    /// <summary>
    /// Мөнгө гарах дансны дугаар.
    /// Энэ дансны үлдэгдэл хасагдана.
    /// </summary>
    public string FromAccount { get; set; } = string.Empty;

    /// <summary>
    /// Мөнгө орох дансны дугаар.
    /// Энэ дансны үлдэгдэл нэмэгдэнэ.
    /// </summary>
    public string ToAccount { get; set; } = string.Empty;

    /// <summary>
    /// Шилжүүлэх мөнгөн дүн.
    /// 0-с их байх ёстой — TransactionService шалгана.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>Гүйлгээ хийгдсэн цаг (UTC)</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Гүйлгээ амжилттай болсон эсэх.
    /// false: үлдэгдэл хүрэлцэхгүй эсвэл данс олдсонгүй.
    /// </summary>
    public bool IsSuccess { get; set; }
}