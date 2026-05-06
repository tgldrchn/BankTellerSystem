using BankTeller.Core.Models;

namespace BankTeller.Core.Interfaces;

/// <summary>
/// Хүлээлгийн дугаарын удирдлагын үйлчилгээний гэрээ.
/// Терминал болон Теллерийн апп хоёул энэ interface-г ашиглана.
/// </summary>
public interface IQueueService
{
    /// <summary>
    /// Дараагийн дугаар олгоно.
    /// Терминалаас дуудагдана — үйлчлүүлэгч дугаар авахад.
    /// </summary>
    /// <returns>Шинээр үүсгэсэн QueueTicket</returns>
    Task<QueueTicket> IssueNextAsync();

    /// <summary>
    /// Хүлээж буй дараагийн үйлчлүүлэгчийг дуудна.
    /// Теллерийн аппаас дуудагдана.
    /// </summary>
    /// <returns>Дуудагдсан QueueTicket, эгнээ хоосон бол null</returns>
    Task<QueueTicket?> CallNextAsync();

    /// <summary>
    /// Одоо дуудагдаж буй дугаарыг буцаана.
    /// Дугаар харуулах дэлгэцэд ашиглана.
    /// </summary>
    Task<int> GetCurrentNumberAsync();
}