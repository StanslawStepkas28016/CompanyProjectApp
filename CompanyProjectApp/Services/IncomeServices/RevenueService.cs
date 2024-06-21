using CompanyProjectApp.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace CompanyProjectApp.Services.IncomeServices;

public class RevenueService : IRevenueService
{
    private readonly CompanyProjectAppContext _context;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public RevenueService(CompanyProjectAppContext context, IConfiguration configuration, HttpClient httpClient)
    {
        _context = context;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<double> CalculateActualRevenueForTheWholeCompany(string currencyCode,
        CancellationToken cancellationToken)
    {
        var exchangeRate = await GetExchangeRate(currencyCode, cancellationToken);

        var res
            = await _context
                .Agreements
                .Include(a => a.Payments)
                .Where(a => a.IsSigned == true)
                .Select(a => a.CalculatedPrice)
                .SumAsync(cancellationToken);

        return (double)res * exchangeRate;
    }

    public async Task<double> CalculateExpectedRevenueForTheWholeCompany(string currencyCode,
        CancellationToken cancellationToken)
    {
        var exchangeRate = await GetExchangeRate(currencyCode, cancellationToken);

        var res
            = await _context
                .Agreements
                .Include(a => a.Payments)
                .Select(a => a.CalculatedPrice)
                .SumAsync(cancellationToken);

        return (double)res * exchangeRate;
    }

    public async Task<double> CalculateActualRevenueForAProduct(int productId, string currencyCode,
        CancellationToken cancellationToken)
    {
        var exchangeRate = await GetExchangeRate(currencyCode, cancellationToken);

        var product = await _context
            .Products
            .Where(p => p.IdProduct == productId)
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
        {
            throw new ArgumentException("A product with the provided Id does not exist!");
        }

        var count = await _context
            .Agreements
            .Where(a => a.IdProduct == productId)
            .CountAsync(cancellationToken);

        return (double)(count * product.Price) * exchangeRate;
    }

    private async Task<double> GetExchangeRate(string currencyCode, CancellationToken cancellationToken)
    {
        var apiKey = _configuration["CurrencyAPI:Key"];
        var url = "https://v6.exchangerate-api.com/v6/" + apiKey + "/latest/PLN";

        var responseHttp = await _httpClient.GetAsync(url, cancellationToken);
        responseHttp.EnsureSuccessStatusCode();

        var responseString = await responseHttp.Content.ReadAsStringAsync(cancellationToken);
        var responseJson = JObject.Parse(responseString);

        if (responseJson == null)
        {
            throw new Exception("API call has not been successful, contact the application administrator!");
        }

        if (responseJson["conversion_rates"][currencyCode] == null)
        {
            throw new ArgumentException("The provided currency code is incorrect!");
        }

        var exchangeRate = (double)responseJson["conversion_rates"][currencyCode];

        return exchangeRate;
    }
}