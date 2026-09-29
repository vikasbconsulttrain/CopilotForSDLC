using LoanApplication.Api.Contracts;
using LoanApplication.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanApplication.Api.Controllers;

[ApiController]
[Route("api/loans/{loanId:int}/prepayments")]
public sealed class LoanPrepaymentsController : ControllerBase
{
    private readonly ILoanPrepaymentService _service;

    public LoanPrepaymentsController(ILoanPrepaymentService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType(typeof(LoanPrepaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanPrepaymentResponse>> CreateAsync(
        int loanId,
        [FromBody] CreateLoanPrepaymentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _service.PrepayAsync(loanId, request, cancellationToken).ConfigureAwait(false);
            return response is null ? NotFound() : Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
