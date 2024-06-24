namespace CompanyProjectApp.Services.RevenueServices;

public interface IRevenueService
{
    public Task<double> CalculateActualRevenueForTheWholeCompany(string currencyCode,
        CancellationToken cancellationToken);

    public Task<double> CalculateExpectedRevenueForTheWholeCompany(string currencyCode,
        CancellationToken cancellationToken);

    public Task<double> CalculateActualRevenueForAProduct(int productId, string currencyCode,
        CancellationToken cancellationToken);

    public Task<double> CalculateExpectedRevenueForAProduct(int productId, string currencyCode,
        CancellationToken cancellationToken);
}