using BankTeller.Core.DTOs;
using BankTeller.Core.Interfaces;
using BankTeller.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace BankTeller.TellerApp.Services;

/// <summary>
/// API-тай холбогдох валютын үйлчилгээ.
/// appsettings.json-с холболтын хаягийг уншина.
/// </summary>
public class ApiCurrencyService : ICurrencyService
{
    private readonly HttpClient _http;

    public ApiCurrencyService()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var baseUrl = config["ApiBaseUrl"] ?? "http://localhost:5200";
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<List<CurrencyRate>> GetAllRatesAsync()
    {
        try
        {
            var result = await _http.GetFromJsonAsync<ApiResponse<List<CurrencyRate>>>("/api/currency");
            return result?.Data ?? new List<CurrencyRate>();
        }
        catch { return new List<CurrencyRate>(); }
    }

    public async Task<bool> UpdateRateAsync(string currencyCode, decimal buyRate, decimal sellRate)
    {
        try
        {
            var response = await _http.PutAsJsonAsync("/api/currency", new UpdateRateRequest
            {
                CurrencyCode = currencyCode,
                BuyRate = buyRate,
                SellRate = sellRate
            });
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return result?.Success ?? false;
        }
        catch { return false; }
    }
}