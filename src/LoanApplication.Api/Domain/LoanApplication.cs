namespace LoanApplication.Api.Domain;

public sealed class LoanApplication
{
    public Guid Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string ExistingAccountId { get; set; } = string.Empty;

    public string LoanRequirement { get; set; } = string.Empty;

    public LoanApplicationStatus Status { get; set; }

    public DateTimeOffset SubmittedAtUtc { get; set; }
}
