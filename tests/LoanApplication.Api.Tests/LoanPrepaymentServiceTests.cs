using LoanApplication.Api.Contracts;
using LoanApplication.Api.Orm;
using LoanApplication.Api.Orm.Entities;
using LoanApplication.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Api.Tests;

public sealed class LoanPrepaymentServiceTests
{
    [Fact]
    public async Task PrepayAsync_WithFullAmount_MarksLoanPaidOff()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Loans.Add(CreateApprovedLoan(1001, 50000m, 1250m));
        await dbContext.SaveChangesAsync();

        var service = new LoanPrepaymentService(dbContext);

        var response = await service.PrepayAsync(
            1001,
            new CreateLoanPrepaymentRequest
            {
                PrepaymentAmount = 1250m
            });

        Assert.NotNull(response);
        Assert.Equal(1001, response!.LoanId);
        Assert.Equal(1250m, response.PrepaymentAmount);
        Assert.Equal(50000m, response.OriginalLoanAmount);
        Assert.Equal(0m, response.RemainingPrincipal);
        Assert.Equal("PaidOff", response.LoanStatus);
        Assert.NotNull(response.PaidOffAtUtc);

        var persisted = await dbContext.Loans.SingleAsync(entity => entity.LoanId == 1001);
        Assert.Equal(0m, persisted.RemainingPrincipal);
        Assert.Equal("PaidOff", persisted.LoanStatus);
        Assert.NotNull(persisted.PaidOffAtUtc);
    }

    [Fact]
    public async Task PrepayAsync_WithPartialAmount_ReducesRemainingPrincipal()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Loans.Add(CreateApprovedLoan(1002, 80000m, 5000m));
        await dbContext.SaveChangesAsync();

        var service = new LoanPrepaymentService(dbContext);

        var response = await service.PrepayAsync(
            1002,
            new CreateLoanPrepaymentRequest
            {
                PrepaymentAmount = 1500m
            });

        Assert.NotNull(response);
        Assert.Equal(3500m, response!.RemainingPrincipal);
        Assert.Equal("Approved", response.LoanStatus);
        Assert.Null(response.PaidOffAtUtc);

        var persisted = await dbContext.Loans.SingleAsync(entity => entity.LoanId == 1002);
        Assert.Equal(3500m, persisted.RemainingPrincipal);
        Assert.Equal("Approved", persisted.LoanStatus);
        Assert.Null(persisted.PaidOffAtUtc);
    }

    [Fact]
    public async Task PrepayAsync_ForUnknownLoan_ReturnsNull()
    {
        await using var dbContext = CreateDbContext();
        var service = new LoanPrepaymentService(dbContext);

        var response = await service.PrepayAsync(
            9999,
            new CreateLoanPrepaymentRequest
            {
                PrepaymentAmount = 100m
            });

        Assert.Null(response);
    }

    [Fact]
    public async Task PrepayAsync_WithAmountExceedingRemainingPrincipal_ThrowsInvalidOperationException()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Loans.Add(CreateApprovedLoan(1003, 25000m, 1000m));
        await dbContext.SaveChangesAsync();

        var service = new LoanPrepaymentService(dbContext);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.PrepayAsync(
            1003,
            new CreateLoanPrepaymentRequest
            {
                PrepaymentAmount = 1001m
            }));

        Assert.Equal("Prepayment amount cannot exceed the remaining principal.", exception.Message);
    }

    private static BankingDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BankingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new BankingDbContext(options);
    }

    private static Loan CreateApprovedLoan(int loanId, decimal originalAmount, decimal remainingPrincipal)
    {
        return new Loan
        {
            LoanId = loanId,
            CustomerId = 1,
            BranchId = 1,
            LoanAmount = originalAmount,
            RemainingPrincipal = remainingPrincipal,
            InterestRate = 7.25m,
            LoanStatus = "Approved",
            ApplicationDate = new DateTime(2025, 1, 1)
        };
    }
}
