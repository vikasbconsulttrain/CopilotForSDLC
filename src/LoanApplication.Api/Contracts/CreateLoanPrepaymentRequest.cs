using System.ComponentModel.DataAnnotations;

namespace LoanApplication.Api.Contracts;

public sealed class CreateLoanPrepaymentRequest
{
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal PrepaymentAmount { get; init; }
}
