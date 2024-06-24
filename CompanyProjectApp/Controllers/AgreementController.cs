using CompanyProjectApp.Dtos.AgreementDtos;
using CompanyProjectApp.Dtos.ProductClientDtos;
using CompanyProjectApp.Services.AgreementServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyProjectApp.Controllers;

/// <summary>
///     Controller responsible for presenting data from two specified use cases - CreateAgreement and PayForAgreement.
/// </summary>
[Route("api/agreements")]
[ApiController]
public class AgreementController : ControllerBase
{
    private readonly IAgreementService _service;

    public AgreementController(IAgreementService service)
    {
        _service = service;
    }

    /// <summary>
    ///     Endpoint used for creating and agreement. The input data is being validated to match the complicated business logic.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin,client,worker")]
    [HttpPost]
    public async Task<IActionResult> CreateAgreement(CreateAgreementRequestDto request,
        CancellationToken cancellationToken)
    {
        var res = await _service.CreateAgreement(request, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    ///     Endpoint used for paying for the agreement. A user can pay the full price or partially
    ///     (it is all included in the response with Fields: MoneyPaid, MoneyOwed).
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin,client")]
    [HttpPost("pay")]
    public async Task<IActionResult> PayForAgreement(PayForAgreementRequestDto request,
        CancellationToken cancellationToken)
    {
        var res = await _service.PayForAgreement(request, cancellationToken);
        return Ok(res);
    }
}