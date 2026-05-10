using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Http.Json;
using BankTeller.Core.DTOs;
using BankTeller.Core.Models;

namespace BankTeller.NumberTerminal.Services;

public class ApiQueueService : IQueueClient
{
    private readonly HttpClient _httpClient;

    public ApiQueueService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7001")
        };
    }

    public async Task<QueueTicket> IssueTicketAsync()
    {
        var response = await _httpClient.PostAsync("/api/queue/issue", null);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<QueueTicket>>();

        if (result == null || result.Data == null)
        {
            throw new Exception("API-аас дугаарын мэдээлэл ирсэнгүй.");
        }

        return result.Data;
    }
}