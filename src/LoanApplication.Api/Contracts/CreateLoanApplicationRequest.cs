using System.ComponentModel.DataAnnotations;

namespace LoanApplication.Api.Contracts;

public sealed class CreateLoanApplicationRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string CustomerName { get; init; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string ExistingAccountId { get; init; } = string.Empty;

    [Required]
    [StringLength(1000, MinimumLength = 10)]
    public string LoanRequirement { get; init; } = string.Empty;
}
