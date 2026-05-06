using BankTeller.Core.Enums;

namespace BankTeller.Core.Models;

/// <summary>
/// Банкны үүдэнд олгодог хүлээлгийн дугаарын тасалбар.
/// Үйлчлүүлэгч терминалаас дугаар авахад үүсдэг.
/// </summary>
public class QueueTicket
{
    /// <summary>Өгөгдлийн сангийн өвөрмөц дугаар</summary>
    public int Id { get; set; }

    /// <summary>
    /// Үйлчлүүлэгчид олгосон дарааллын дугаар.
    /// Жишээ: 1, 2, 3 ... өдөр бүр 1-ээс эхэлнэ.
    /// </summary>
    public int Number { get; set; }

    /// <summary>Тасалбар олгосон цаг (UTC)</summary>
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Тасалбарын одоогийн төлөв.
    /// Анхны утга: Waiting (хүлээж байна)
    /// </summary>
    public TicketStatus Status { get; set; } = TicketStatus.Waiting;
}