using LoanApplication.Api.Orm.Contracts;
using LoanApplication.Api.Orm.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanApplication.Api.Controllers;

[ApiController]
[Route("api/lending")]
public sealed class LendingQueriesController : ControllerBase
{
    private readonly ILendingQueryService _service;
    private readonly ILogger<LendingQueriesController> _logger;

    public LendingQueriesController(ILendingQueryService service, ILogger<LendingQueriesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("approved-loans")]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerLoanTransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<CustomerLoanTransactionDto>>> GetApprovedLoansAsync(
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        if (year is < 2000 or > 2100)
        {
            return BadRequest("Year must be between 2000 and 2100.");
        }

        try
        {
            var result = await _service
                .GetApprovedLoansWithTransactionsAsync(year, cancellationToken)
                .ConfigureAwait(false);

            return Ok(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to retrieve lending query result for year {Year}.", year);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }
}
