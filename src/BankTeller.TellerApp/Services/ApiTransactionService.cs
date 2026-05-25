using BankTeller.Core.DTOs;
using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace BankTeller.TellerApp.Services;

/// <summary>
/// API-тай холбогдох гүйлгээний үйлчилгээ.
/// appsettings.json-с холболтын хаягийг уншина.
/// </summary>
public class ApiTransactionService : ITransactionService
{
    private readonly HttpClient _http;

    public ApiTransactionService()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var baseUrl = config["ApiBaseUrl"] ?? "http://localhost:5200";
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<Account?> GetAccountAsync(string accountNumber)
    {
        try
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<Account>>(
                $"/api/transaction/account/{accountNumber}");
            return result?.Data;
        }
        catch { return null; }
    }

    public async Task<bool> TransferAsync(string fromAccount, string toAccount, decimal amount)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("/api/transaction/transfer", new TransferRequest
            {
                FromAccount = fromAccount,
                ToAccount = toAccount,
                Amount = amount
            });
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return result?.Success ?? false;
        }
        catch { return false; }
    }
}