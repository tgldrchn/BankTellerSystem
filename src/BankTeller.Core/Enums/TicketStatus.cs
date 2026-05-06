namespace BankTeller.Core.Enums;

/// <summary>
/// Дугаарын тасалбарын төлөв байдал
/// </summary>
public enum TicketStatus
{
    /// <summary>Үйлчлүүлэгч хүлээлгийн эгнээнд байна</summary>
    Waiting,

    /// <summary>Теллер дуудсан, үйлчлүүлэгч ирэх ёстой</summary>
    Called,

    /// <summary>Үйлчилгээ дууссан</summary>
    Done
}