// DTOs/TransferRequest.cs
// Теллер мөнгө шилжүүлэх үед API-д явуулах өгөгдөл
namespace BankTeller.Core.DTOs;

/// <summary>
/// Теллерийн аппаас мөнгө шилжүүлэх хүсэлтийн өгөгдөл
/// </summary>
public class TransferRequest
{
    /// <summary>Мөнгө гарах дансны дугаар</summary>
    public string FromAccount { get; set; } = string.Empty;

    /// <summary>Мөнгө орох дансны дугаар</summary>
    public string ToAccount { get; set; } = string.Empty;

    /// <summary>Шилжүүлэх дүн</summary>
    public decimal Amount { get; set; }
}