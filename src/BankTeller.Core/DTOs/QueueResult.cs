// BankTeller.Core/DTOs/QueueResults.cs
namespace BankTeller.Core.DTOs;

/// <summary>Одоогийн дуудагдсан дугаарын хариу</summary>
public class NumberResult
{
    public int Number { get; set; }
}

/// <summary>Хүлээж буй үйлчлүүлэгчийн тооны хариу</summary>
public class CountResult
{
    public int Count { get; set; }
}