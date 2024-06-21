using CompanyProjectApp.Dtos.ClientManagementDtos;
using CompanyProjectApp.Services.ClientRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyProjectApp.Controllers;

/// <summary>
///     Controller responsible for providing functionalities required for managing the app users.
/// </summary>
[Route("api/clients")]
[ApiController]
public class ClientManagementController : ControllerBase
{
    private IClientManagementService _clientManagementService;

    public ClientManagementController(IClientManagementService clientManagementService)
    {
        _clientManagementService = clientManagementService;
    }

    /// <summary>
    ///     Endpoint used for adding a new physical client. Available to any logged user.
    /// </summary>
    /// <param name="physicalClient"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin,regular")]
    [HttpPost("physical")]
    public async Task<IActionResult> AddNewPhysicalClient(RegisterPhysicalClientDto physicalClient,
        CancellationToken cancellationToken)
    {
        var res = await _clientManagementService.AddNewPhysicalClient(physicalClient, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    ///     Endpoint used for modifying a physical client data. Available to the admin only. 
    /// </summary>
    /// <param name="physicalClientPesel"></param>
    /// <param name="physicalClientDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin")]
    [HttpPut("physical/{physicalClientPesel}")]
    public async Task<IActionResult> ModifyPhysicalClient(string physicalClientPesel,
        ModifyPhysicalClientDto physicalClientDto,
        CancellationToken cancellationToken)
    {
        var res = await _clientManagementService.ModifyPhysicalClient(physicalClientPesel, physicalClientDto,
            cancellationToken);
        return Ok(res);
    }

    /// <summary>
    ///     Endpoint used for deleting (shallow deleting) a physical client data. Available for the admin only. 
    /// </summary>
    /// <param name="physicalClientPesel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin")]
    [HttpDelete("physical/{physicalClientPesel}")]
    public async Task<IActionResult> DeletePhysicalClient(string physicalClientPesel,
        CancellationToken cancellationToken)
    {
        await _clientManagementService.DeletePhysicalClient(physicalClientPesel, cancellationToken);
        return Ok("Client successfully deleted");
    }

    /// <summary>
    ///     Endpoint used for adding a company client. Available to any logged user.
    /// </summary>
    /// <param name="companyClient"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin,regular")]
    [HttpPost("company")]
    public async Task<IActionResult> AddNewCompanyClient(RegisterCompanyClientDto companyClient,
        CancellationToken cancellationToken)
    {
        var res = await _clientManagementService.AddNewCompanyClient(companyClient, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    ///      Endpoint used for modifying a company client data. Available to any logged user.
    /// </summary>
    /// <param name="krsNumber"></param>
    /// <param name="companyClient"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(Roles = "admin,regular")]
    [HttpPut("company/{krsNumber}")]
    public async Task<IActionResult> ModifyCompanyClient(string krsNumber, ModifyCompanyClientDto companyClient,
        CancellationToken cancellationToken)
    {
        var res = await _clientManagementService.ModifyCompanyClient(krsNumber, companyClient, cancellationToken);
        return Ok(res);
    }
}