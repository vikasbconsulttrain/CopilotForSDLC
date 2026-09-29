namespace LoanApplication.Api.Contracts;

public sealed class LoanPrepaymentResponse
{
    public int LoanId { get; init; }

    public decimal PrepaymentAmount { get; init; }

    public decimal OriginalLoanAmount { get; init; }

    public decimal RemainingPrincipal { get; init; }

    public string LoanStatus { get; init; } = string.Empty;

    public DateTimeOffset? PaidOffAtUtc { get; init; }
}
