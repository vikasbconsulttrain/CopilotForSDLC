using LoanApplication.Api.Contracts;
using LoanApplication.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanApplication.Api.Controllers;

[ApiController]
[Route("api/loan-applications")]
public sealed class LoanApplicationsController : ControllerBase
{
    private readonly ILoanApplicationService _service;

    public LoanApplicationsController(ILoanApplicationService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(typeof(LoanApplicationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoanApplicationResponse>> CreateAsync(
        [FromBody] CreateLoanApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _service.SubmitAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = response.ApplicationId }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LoanApplicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanApplicationResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await _service.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return response is null ? NotFound() : Ok(response);
    }
}
