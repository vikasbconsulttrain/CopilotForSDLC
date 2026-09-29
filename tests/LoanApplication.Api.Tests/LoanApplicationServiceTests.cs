using LoanApplication.Api.Contracts;
using LoanApplication.Api.Repositories;
using LoanApplication.Api.Services;

namespace LoanApplication.Api.Tests;

public sealed class LoanApplicationServiceTests
{
    [Fact]
    public async Task SubmitAsync_PersistsApplicationAndTrimsInput()
    {
        var repository = new InMemoryLoanApplicationRepository();
        var service = new LoanApplicationService(repository);

        var response = await service.SubmitAsync(new CreateLoanApplicationRequest
        {
            CustomerName = "  Jane Doe  ",
            ExistingAccountId = "  ACC-12345  ",
            LoanRequirement = "  Personal loan for renovation  "
        });

        Assert.NotEqual(Guid.Empty, response.ApplicationId);
        Assert.Equal("Jane Doe", response.CustomerName);
        Assert.Equal("ACC-12345", response.ExistingAccountId);
        Assert.Equal("Personal loan for renovation", response.LoanRequirement);
        Assert.Equal("Submitted", response.Status);
        Assert.NotEqual(default, response.SubmittedAtUtc);

        var persisted = await service.GetByIdAsync(response.ApplicationId);

        Assert.NotNull(persisted);
        Assert.Equal(response.ApplicationId, persisted!.ApplicationId);
        Assert.Equal(response.CustomerName, persisted.CustomerName);
    }

    [Fact]
    public async Task GetByIdAsync_ForUnknownApplication_ReturnsNull()
    {
        var service = new LoanApplicationService(new InMemoryLoanApplicationRepository());

        var response = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(response);
    }
}
