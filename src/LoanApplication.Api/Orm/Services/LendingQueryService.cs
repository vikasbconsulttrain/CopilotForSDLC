using LoanApplication.Api.Orm.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LoanApplication.Api.Orm.Services;

public sealed class LendingQueryService : ILendingQueryService
{
    private readonly BankingDbContext _dbContext;

    public LendingQueryService(BankingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CustomerLoanTransactionDto>> GetApprovedLoansWithTransactionsAsync(
        int year,
        CancellationToken cancellationToken = default)
    {
        var start = new DateTime(year, 1, 1);
        var end = start.AddYears(1);

        var query =
            from customer in _dbContext.Customers.AsNoTracking()
            from loan in customer.Loans
                .Where(loan => loan.ApplicationDate >= start
                    && loan.ApplicationDate < end
                    && loan.LoanStatus == "Approved")
            where customer.Transactions.Any(transaction => transaction.TransactionAmount > 10_000)
            from transaction in customer.Transactions.DefaultIfEmpty()
            orderby customer.LastName, customer.FirstName, loan.ApplicationDate descending
            select new CustomerLoanTransactionDto
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                DateOfBirth = customer.DateOfBirth,
                LoanId = loan.LoanId,
                LoanAmount = loan.LoanAmount,
                InterestRate = loan.InterestRate,
                LoanStatus = loan.LoanStatus,
                ApplicationDate = loan.ApplicationDate,
                BranchName = loan.Branch.BranchName,
                BranchAddress = loan.Branch.BranchAddress,
                TransactionId = transaction != null ? transaction.TransactionId : null,
                TransactionDate = transaction != null ? transaction.TransactionDate : null,
                TransactionAmount = transaction != null ? transaction.TransactionAmount : null,
                TransactionType = transaction != null ? transaction.TransactionType : null
            };

        return await query.ToListAsync(cancellationToken);
    }
}
