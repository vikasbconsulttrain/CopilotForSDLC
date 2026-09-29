namespace LoanApplication.Api.Orm.Entities;

public sealed class Loan
{
    public int LoanId { get; set; }
    public int CustomerId { get; set; }
    public int BranchId { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal RemainingPrincipal { get; set; }
    public decimal InterestRate { get; set; }
    public string LoanStatus { get; set; } = string.Empty;
    public DateTime ApplicationDate { get; set; }
    public DateTimeOffset? PaidOffAtUtc { get; set; }

    public Customer Customer { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
}
