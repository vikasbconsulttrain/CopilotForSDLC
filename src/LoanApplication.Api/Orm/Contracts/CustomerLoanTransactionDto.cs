namespace LoanApplication.Api.Orm.Contracts;

public sealed class CustomerLoanTransactionDto
{
    public int CustomerId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }

    public int LoanId { get; init; }
    public decimal LoanAmount { get; init; }
    public decimal InterestRate { get; init; }
    public string LoanStatus { get; init; } = string.Empty;
    public DateTime ApplicationDate { get; init; }

    public string BranchName { get; init; } = string.Empty;
    public string BranchAddress { get; init; } = string.Empty;

    public int? TransactionId { get; init; }
    public DateTime? TransactionDate { get; init; }
    public decimal? TransactionAmount { get; init; }
    public string? TransactionType { get; init; }
}
