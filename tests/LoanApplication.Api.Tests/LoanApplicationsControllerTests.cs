using LoanApplication.Api.Contracts;
using LoanApplication.Api.Controllers;
using LoanApplication.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanApplication.Api.Tests;

public sealed class LoanApplicationsControllerTests
{
    [Fact]
    public async Task CreateAsync_ReturnsCreatedAtActionWithApplicationResponse()
    {
        var expected = new LoanApplicationResponse
        {
            ApplicationId = Guid.NewGuid(),
            CustomerName = "Jane Doe",
            ExistingAccountId = "ACC-12345",
            LoanRequirement = "Personal loan",
            Status = "Submitted",
            SubmittedAtUtc = DateTimeOffset.UtcNow
        };

        var controller = new LoanApplicationsController(new FakeLoanApplicationService(expected, null));

        var result = await controller.CreateAsync(new CreateLoanApplicationRequest
        {
            CustomerName = expected.CustomerName,
            ExistingAccountId = expected.ExistingAccountId,
            LoanRequirement = expected.LoanRequirement
        }, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(LoanApplicationsController.GetByIdAsync), created.ActionName);
        Assert.Equal(expected.ApplicationId, created.RouteValues!["id"]);
        Assert.Same(expected, created.Value);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNotFoundWhenApplicationDoesNotExist()
    {
        var controller = new LoanApplicationsController(new FakeLoanApplicationService(null, null));

        var result = await controller.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    private sealed class FakeLoanApplicationService : ILoanApplicationService
    {
        private readonly LoanApplicationResponse? _submitResponse;
        private readonly LoanApplicationResponse? _getResponse;

        public FakeLoanApplicationService(LoanApplicationResponse? submitResponse, LoanApplicationResponse? getResponse)
        {
            _submitResponse = submitResponse;
            _getResponse = getResponse;
        }

        public Task<LoanApplicationResponse> SubmitAsync(CreateLoanApplicationRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_submitResponse ?? new LoanApplicationResponse
            {
                ApplicationId = Guid.NewGuid(),
                CustomerName = request.CustomerName,
                ExistingAccountId = request.ExistingAccountId,
                LoanRequirement = request.LoanRequirement,
                Status = "Submitted",
                SubmittedAtUtc = DateTimeOffset.UtcNow
            });
        }

        public Task<LoanApplicationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_getResponse);
        }
    }
}
