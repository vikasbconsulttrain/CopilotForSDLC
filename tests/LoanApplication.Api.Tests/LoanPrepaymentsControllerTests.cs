using LoanApplication.Api.Contracts;
using LoanApplication.Api.Controllers;
using LoanApplication.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanApplication.Api.Tests;

public sealed class LoanPrepaymentsControllerTests
{
    [Fact]
    public async Task CreateAsync_WithValidPrepayment_ReturnsOk()
    {
        var expected = new LoanPrepaymentResponse
        {
            LoanId = 1001,
            PrepaymentAmount = 250m,
            OriginalLoanAmount = 50000m,
            RemainingPrincipal = 4750m,
            LoanStatus = "Approved"
        };

        var controller = new LoanPrepaymentsController(new FakeLoanPrepaymentService(expected, null));

        var result = await controller.CreateAsync(
            1001,
            new CreateLoanPrepaymentRequest
            {
                PrepaymentAmount = 250m
            },
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public async Task CreateAsync_WhenLoanDoesNotExist_ReturnsNotFound()
    {
        var controller = new LoanPrepaymentsController(new FakeLoanPrepaymentService(null, null));

        var result = await controller.CreateAsync(
            1001,
            new CreateLoanPrepaymentRequest
            {
                PrepaymentAmount = 250m
            },
            CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateAsync_WhenServiceRejectsRequest_ReturnsBadRequest()
    {
        var controller = new LoanPrepaymentsController(new ThrowingLoanPrepaymentService());

        var result = await controller.CreateAsync(
            1001,
            new CreateLoanPrepaymentRequest
            {
                PrepaymentAmount = 250m
            },
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Prepayment amount cannot exceed the remaining principal.", badRequest.Value);
    }

    private sealed class FakeLoanPrepaymentService : ILoanPrepaymentService
    {
        private readonly LoanPrepaymentResponse? _response;
        private readonly LoanPrepaymentResponse? _notFoundResponse;

        public FakeLoanPrepaymentService(LoanPrepaymentResponse? response, LoanPrepaymentResponse? notFoundResponse)
        {
            _response = response;
            _notFoundResponse = notFoundResponse;
        }

        public Task<LoanPrepaymentResponse?> PrepayAsync(int loanId, CreateLoanPrepaymentRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_response ?? _notFoundResponse);
        }
    }

    private sealed class ThrowingLoanPrepaymentService : ILoanPrepaymentService
    {
        public Task<LoanPrepaymentResponse?> PrepayAsync(int loanId, CreateLoanPrepaymentRequest request, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Prepayment amount cannot exceed the remaining principal.");
        }
    }
}
