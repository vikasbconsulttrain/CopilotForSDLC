using LoanApplication.Api.Contracts;

namespace LoanApplication.Api.Services;

public interface ILoanPrepaymentService
{
    Task<LoanPrepaymentResponse?> PrepayAsync(int loanId, CreateLoanPrepaymentRequest request, CancellationToken cancellationToken = default);
}
