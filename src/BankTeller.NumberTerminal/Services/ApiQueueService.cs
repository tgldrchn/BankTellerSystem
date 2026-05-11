using BankTeller.Core.DTOs;
using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;
using System.Net.Http.Json;

namespace BankTeller.NumberTerminal.Services;

/// <summary>
/// API-тай холбогдох дугаарын үйлчилгээ.
/// </summary>
public class ApiQueueService : IQueueService
{
    private readonly HttpClient _http;

    public ApiQueueService()
    {
        _http = new HttpClient { BaseAddress = new Uri("http://localhost:5200") };
    }

    public async Task<QueueTicket> IssueNextAsync()
    {
        try
        {
            var response = await _http.PostAsync("/api/queue/issue", null);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<QueueTicket>>();
            return result?.Data ?? new QueueTicket();
        }
        catch { return new QueueTicket(); }
    }

    public async Task<QueueTicket?> CallNextAsync()
    {
        try
        {
            var response = await _http.PostAsync("/api/queue/call-next", null);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<QueueTicket>>();
            return result?.Data;
        }
        catch { return null; }
    }

    public async Task<int> GetCurrentNumberAsync()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<NumberResult>>("/api/queue/current");
            return result?.Data?.Number ?? 0;
        }
        catch { return 0; }
    }

    public async Task<int> GetWaitingCountAsync()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<CountResult>>("/api/queue/waiting-count");
            return result?.Data?.Count ?? 0;
        }
        catch { return 0; }
    }
}