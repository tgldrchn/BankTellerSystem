using BankTeller.Core.Models;

namespace BankTeller.Core.Interfaces;

/// <summary>
/// Мөнгөн гүйлгээний үйлчилгээний гэрээ.
/// Теллерийн апп энэ interface-г ашиглана.
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Дансны мэдээллийг дансны дугаараар хайна.
    /// </summary>
    /// <param name="accountNumber">Хайх дансны дугаар. Жишээ: "ACC001"</param>
    /// <returns>Олдвол Account, олдохгүй бол null</returns>
    Task<Account?> GetAccountAsync(string accountNumber);

    /// <summary>
    /// А данснаас Б данс руу мөнгө шилжүүлнэ.
    /// Давхар гүйлгээнээс сэргийлэхийн тулд Queue-гаар дамжина.
    /// </summary>
    /// <param name="fromAccount">Мөнгө гарах дансны дугаар</param>
    /// <param name="toAccount">Мөнгө орох дансны дугаар</param>
    /// <param name="amount">Шилжүүлэх дүн (0-с их байх ёстой)</param>
    /// <returns>Амжилттай бол true, үлдэгдэл хүрэлцэхгүй/данс олдохгүй бол false</returns>
    Task<bool> TransferAsync(string fromAccount, string toAccount, decimal amount);
}