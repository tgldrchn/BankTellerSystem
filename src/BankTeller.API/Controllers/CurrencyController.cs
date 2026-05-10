using BankTeller.API.Hubs;
using BankTeller.Core.DTOs;
using BankTeller.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BankTeller.API.Controllers;

[ApiController]
[Route("api/currency")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _currency;
    private readonly IHubContext<BankHub> _hub;

    public CurrencyController(ICurrencyService currency, IHubContext<BankHub> hub)
    {
        _currency = currency;
        _hub = hub;
    }

    /// <summary>Бүх ханш авах</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll()
    {
        var rates = await _currency.GetAllRatesAsync();
        return Ok(ApiResponse<object>.Ok(rates));
    }

    /// <summary>Ханш өөрчлөх — Теллер дуудна</summary>
    [HttpPut]
    public async Task<ActionResult<ApiResponse<object>>> Update([FromBody] UpdateRateRequest request)
    {
        var success = await _currency.UpdateRateAsync(
            request.CurrencyCode, request.BuyRate, request.SellRate);

        if (!success)
            return BadRequest(ApiResponse<object>.Fail("Ханш өөрчлөх амжилтгүй"));

        await _hub.Clients.All.SendAsync("RateUpdated");
        return Ok(ApiResponse<object>.Ok(new { }, "Ханш шинэчлэгдлээ"));
    }
}