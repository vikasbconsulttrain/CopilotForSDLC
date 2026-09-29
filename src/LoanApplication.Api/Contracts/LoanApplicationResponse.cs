namespace LoanApplication.Api.Contracts;

public sealed class LoanApplicationResponse
{
    public Guid ApplicationId { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public string ExistingAccountId { get; init; } = string.Empty;

    public string LoanRequirement { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public DateTimeOffset SubmittedAtUtc { get; init; }
}
