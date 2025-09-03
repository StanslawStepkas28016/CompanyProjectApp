using CompanyProjectApp.Services.RevenueServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyProjectApp.Controllers;

/// <summary>
///     Controller responsible for calculating the revenue of the company and products.
///     Providing functionalities for calculating the expected revenue as well as the actual revenue. 
/// </summary>
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
    [Authorize(Roles = "admin,employee")]
    [HttpGet("company/actual/{currencyCode}")]
    public async Task<IActionResult> CalculateActualRevenueForTheWholeCompany(string currencyCode,
        CancellationToken cancellationToken)
    {
        var res = await _service.CalculateActualRevenueForTheWholeCompany(currencyCode, cancellationToken);
        return Ok("Actual revenue for the company is equal to = " + res);
    }

    /// <summary>
    ///     Endpoint used for calculating the expected revenue of the whole company 
    ///     (expected, meaning the money sum of signed and unsigned agreements).
    /// </summary>
    /// <param name="currencyCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin,employee")]
    [HttpGet("company/expected/{currencyCode}")]
    public async Task<IActionResult> CalculateExpectedRevenueForTheWholeCompany(string currencyCode,
        CancellationToken cancellationToken)
    {
        var res = await _service.CalculateExpectedRevenueForTheWholeCompany(currencyCode, cancellationToken);
        return Ok("Expected revenue for the company is equal to = " + res);
    }

    /// <summary>
    ///     Endpoint used for calculating the actual revenue of a product which the company sells 
    ///     (actual, meaning the money sum of signed and agreements for a specified product).
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="currencyCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin,employee")]
    [HttpGet("products/{productId:int}/actual/{currencyCode}")]
    public async Task<IActionResult> CalculateActualRevenueForAProduct(int productId, string currencyCode,
        CancellationToken cancellationToken)
    {
        var res = await _service.CalculateActualRevenueForAProduct(productId, currencyCode, cancellationToken);
        return Ok("Actual revenue for a product with IdProduct = " + productId + ", is equal to = " + res);
    }

    /// <summary>
    ///     Endpoint used for calculating the expected revenue of a product which the company sells 
    ///     (expected, meaning the money sum of signed and unsigned agreements for a specified product).
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="currencyCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin,employee")]
    [HttpGet("products/{productId:int}/expected/{currencyCode}")]
    public async Task<IActionResult> CalculateActualExpectedForAProduct(int productId, string currencyCode,
        CancellationToken cancellationToken)
    {
        var res = await _service.CalculateExpectedRevenueForAProduct(productId, currencyCode, cancellationToken);
        return Ok("Expected revenue for a product with IdProduct = " + productId + ", is equal to = " + res);
    }
}