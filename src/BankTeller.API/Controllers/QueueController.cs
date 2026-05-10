using BankTeller.API.Hubs;
using BankTeller.Core.DTOs;
using BankTeller.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BankTeller.API.Controllers;

[ApiController]
[Route("api/queue")]
public class QueueController : ControllerBase
{
    private readonly IQueueService _queue;
    private readonly IHubContext<BankHub> _hub;

    public QueueController(IQueueService queue, IHubContext<BankHub> hub)
    {
        _queue = queue;
        _hub = hub;
    }

    /// <summary>Терминалаас шинэ дугаар авах</summary>
    [HttpPost("issue")]
    public async Task<ActionResult<ApiResponse<object>>> Issue()
    {
        var ticket = await _queue.IssueNextAsync();
        return Ok(ApiResponse<object>.Ok(new
        {
            ticket.Number,
            ticket.IssuedAt
        }));
    }

    /// <summary>Теллер дараагийн үйлчлүүлэгч дуудах</summary>
    [HttpPost("call-next")]
    public async Task<ActionResult<ApiResponse<object>>> CallNext()
    {
        var ticket = await _queue.CallNextAsync();
        if (ticket == null)
            return Ok(ApiResponse<object>.Fail("Хүлээж буй үйлчлүүлэгч байхгүй"));

        await _hub.Clients.All.SendAsync("NumberCalled", ticket.Number);
        return Ok(ApiResponse<object>.Ok(new { ticket.Number }));
    }

    /// <summary>Одоогийн дуудагдсан дугаар авах</summary>
    [HttpGet("current")]
    public async Task<ActionResult<ApiResponse<object>>> Current()
    {
        var number = await _queue.GetCurrentNumberAsync();
        return Ok(ApiResponse<object>.Ok(new { Number = number }));
    }

    /// <summary>Хүлээж буй үйлчлүүлэгчийн тоо авах</summary>
    [HttpGet("waiting-count")]
    public async Task<ActionResult<ApiResponse<object>>> WaitingCount()
    {
        var count = await _queue.GetWaitingCountAsync();
        return Ok(ApiResponse<object>.Ok(new { Count = count }));
    }
}