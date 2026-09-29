using LoanApplication.Api.Orm.Contracts;

namespace LoanApplication.Api.Orm.Services;

public interface ILendingQueryService
{
    Task<IReadOnlyList<CustomerLoanTransactionDto>> GetApprovedLoansWithTransactionsAsync(
        int year,
        CancellationToken cancellationToken = default);
}
