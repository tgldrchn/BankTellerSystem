using BankTeller.Core.DTOs;
using BankTeller.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankTeller.API.Controllers;

[ApiController]
[Route("api/transaction")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transaction;

    public TransactionController(ITransactionService transaction) =>
        _transaction = transaction;

    /// <summary>Дансны мэдээлэл авах</summary>
    [HttpGet("account/{accountNumber}")]
    public async Task<ActionResult<ApiResponse<object>>> GetAccount(string accountNumber)
    {
        var acc = await _transaction.GetAccountAsync(accountNumber);
        if (acc == null)
            return NotFound(ApiResponse<object>.Fail("Данс олдсонгүй"));

        return Ok(ApiResponse<object>.Ok(new
        {
            acc.AccountNumber,
            acc.OwnerName,
            acc.Balance,
            acc.Currency
        }));
    }

    /// <summary>Мөнгө шилжүүлэх</summary>
    [HttpPost("transfer")]
    public async Task<ActionResult<ApiResponse<object>>> Transfer([FromBody] TransferRequest request)
    {
        var success = await _transaction.TransferAsync(
            request.FromAccount, request.ToAccount, request.Amount);

        return success
            ? Ok(ApiResponse<object>.Ok(new { }, "Гүйлгээ амжилттай"))
            : BadRequest(ApiResponse<object>.Fail("Гүйлгээ амжилтгүй"));
    }
}