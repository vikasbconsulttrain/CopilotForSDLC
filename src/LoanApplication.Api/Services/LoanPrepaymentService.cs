using LoanApplication.Api.Contracts;
using LoanApplication.Api.Orm;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Api.Services;

public sealed class LoanPrepaymentService : ILoanPrepaymentService
{
    private const string ApprovedStatus = "Approved";
    private const string PaidOffStatus = "PaidOff";

    private readonly BankingDbContext _dbContext;

    public LoanPrepaymentService(BankingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LoanPrepaymentResponse?> PrepayAsync(int loanId, CreateLoanPrepaymentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var loan = await _dbContext.Loans
            .FirstOrDefaultAsync(entity => entity.LoanId == loanId, cancellationToken)
            .ConfigureAwait(false);

        if (loan is null)
        {
            return null;
        }

        if (!string.Equals(loan.LoanStatus, ApprovedStatus, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Loan {loanId} is not eligible for prepayment.");
        }

        var remainingPrincipal = loan.RemainingPrincipal > 0m ? loan.RemainingPrincipal : loan.LoanAmount;

        if (request.PrepaymentAmount > remainingPrincipal)
        {
            throw new InvalidOperationException("Prepayment amount cannot exceed the remaining principal.");
        }

        remainingPrincipal -= request.PrepaymentAmount;
        loan.RemainingPrincipal = remainingPrincipal;

        if (remainingPrincipal == 0m)
        {
            loan.LoanStatus = PaidOffStatus;
            loan.PaidOffAtUtc = DateTimeOffset.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new LoanPrepaymentResponse
        {
            LoanId = loan.LoanId,
            PrepaymentAmount = request.PrepaymentAmount,
            OriginalLoanAmount = loan.LoanAmount,
            RemainingPrincipal = loan.RemainingPrincipal,
            LoanStatus = loan.LoanStatus,
            PaidOffAtUtc = loan.PaidOffAtUtc
        };
    }
}
