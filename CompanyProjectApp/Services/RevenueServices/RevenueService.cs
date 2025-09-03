using CompanyProjectApp.Context;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace CompanyProjectApp.Services.RevenueServices;

public class RevenueService : IRevenueService
{
    private readonly CompanyProjectAppContext _context;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public RevenueService(CompanyProjectAppContext context)
    {
        _context = context;
        _httpClient = new HttpClient();
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    }

    public async Task<double> CalculateActualRevenueForTheWholeCompany(string currencyCode,
        CancellationToken cancellationToken)
    {
        var exchangeRate = await GetExchangeRate(currencyCode, cancellationToken);

        var res
            = await _context
                .Agreements
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
                .Where(a => a.IsSigned == true || a.IsSigned == false)
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

        var sumCalculatedPricesForAProduct = await _context
            .Agreements
            .Where(a => a.IdProduct == productId && a.IsSigned == true)
            .SumAsync(a => a.CalculatedPrice, cancellationToken);

        return (double)sumCalculatedPricesForAProduct * exchangeRate;
    }

    public async Task<double> CalculateExpectedRevenueForAProduct(int productId, string currencyCode,
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

        var sumCalculatedPricesForAProduct = await _context
            .Agreements
            .Where(a => a.IdProduct == productId && (a.IsSigned == true || a.IsSigned == false))
            .SumAsync(a => a.CalculatedPrice, cancellationToken);

        return (double)sumCalculatedPricesForAProduct * exchangeRate;
    }

    public async Task<double> GetExchangeRate(string currencyCode, CancellationToken cancellationToken)
    {
        // Zakładamy, że ceny w bazie, są przechowywane w PLN, więc nie wykonujemy niepotrzebnie zapytania do API.
        if (currencyCode == "PLN")
        {
            return 1;
        }

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