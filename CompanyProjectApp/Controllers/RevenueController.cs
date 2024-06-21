using CompanyProjectApp.Services.IncomeServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyProjectApp.Controllers;

[Route("api/revenues")]
[ApiController]
public class RevenueController : ControllerBase
{
    private readonly IRevenueService _service;

    public RevenueController(IRevenueService service)
    {
        _service = service;
    }

    /// <summary>
    ///     Endpoint used for calculating the actual revenue of the whole company
    ///     (actual, meaning the money sum of signed agreements).
    /// </summary>
    /// <param name="currencyCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin,regular")]
    [HttpGet("company/actual/{currencyCode}")]
    public async Task<IActionResult> CalculateActualRevenueForTheWholeCompany(string currencyCode,
        CancellationToken cancellationToken)
    {
        var res = await _service.CalculateActualRevenueForTheWholeCompany(currencyCode, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    ///     Endpoint used for calculating the expected revenue of the whole company 
    ///     (expected, meaning the money sum of signed and unsigned agreements).
    /// </summary>
    /// <param name="currencyCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin,regular")]
    [HttpGet("company/expected/{currencyCode}")]
    public async Task<IActionResult> CalculateExpectedRevenueForTheWholeCompany(string currencyCode,
        CancellationToken cancellationToken)
    {
        var res = await _service.CalculateExpectedRevenueForTheWholeCompany(currencyCode, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    ///     Endpoint used for calculating the actual revenue of a product which the company sells 
    ///     (actual, meaning the money sum of signed and agreements for a specified product).
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="currencyCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin,regular")]
    [HttpGet("products/{productId:int}/actual/{currencyCode}")]
    public async Task<IActionResult> CalculateActualRevenueForAProduct(int productId, string currencyCode,
        CancellationToken cancellationToken)
    {
        var res = await _service.CalculateActualRevenueForAProduct(productId, currencyCode, cancellationToken);
        return Ok(res);
    }
}