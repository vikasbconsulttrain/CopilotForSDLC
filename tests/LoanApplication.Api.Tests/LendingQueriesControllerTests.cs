using LoanApplication.Api.Controllers;
using LoanApplication.Api.Orm.Contracts;
using LoanApplication.Api.Orm.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace LoanApplication.Api.Tests;

public sealed class LendingQueriesControllerTests
{
    [Fact]
    public async Task GetApprovedLoansAsync_WithValidYear_ReturnsOk()
    {
        var expected = new List<CustomerLoanTransactionDto>
        {
            new()
            {
                CustomerId = 1,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@example.com",
                Phone = "111-111-1111",
                Address = "123 Main Street",
                DateOfBirth = new DateTime(1990, 1, 1),
                LoanId = 10,
                LoanAmount = 50000,
                InterestRate = 7.25m,
                LoanStatus = "Approved",
                ApplicationDate = new DateTime(2025, 3, 1),
                BranchName = "Downtown",
                BranchAddress = "45 Bank Street"
            }
        };

        var controller = new LendingQueriesController(
            new FakeLendingQueryService(expected),
            NullLogger<LendingQueriesController>.Instance);

        var result = await controller.GetApprovedLoansAsync(2025, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var actual = Assert.IsAssignableFrom<IReadOnlyList<CustomerLoanTransactionDto>>(ok.Value);
        Assert.Single(actual);
    }

    [Theory]
    [InlineData(1999)]
    [InlineData(2101)]
    public async Task GetApprovedLoansAsync_WithInvalidYear_ReturnsBadRequest(int year)
    {
        var controller = new LendingQueriesController(
            new FakeLendingQueryService([]),
            NullLogger<LendingQueriesController>.Instance);

        var result = await controller.GetApprovedLoansAsync(year, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Year must be between 2000 and 2100.", badRequest.Value);
    }

    [Fact]
    public async Task GetApprovedLoansAsync_WhenServiceThrows_ReturnsInternalServerError()
    {
        var controller = new LendingQueriesController(
            new ThrowingLendingQueryService(),
            NullLogger<LendingQueriesController>.Instance);

        var result = await controller.GetApprovedLoansAsync(2025, CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal("An unexpected error occurred.", objectResult.Value);
    }

    private sealed class FakeLendingQueryService : ILendingQueryService
    {
        private readonly IReadOnlyList<CustomerLoanTransactionDto> _response;

        public FakeLendingQueryService(IReadOnlyList<CustomerLoanTransactionDto> response)
        {
            _response = response;
        }

        public Task<IReadOnlyList<CustomerLoanTransactionDto>> GetApprovedLoansWithTransactionsAsync(int year, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_response);
        }
    }

    private sealed class ThrowingLendingQueryService : ILendingQueryService
    {
        public Task<IReadOnlyList<CustomerLoanTransactionDto>> GetApprovedLoansWithTransactionsAsync(int year, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Simulated failure");
        }
    }
}
