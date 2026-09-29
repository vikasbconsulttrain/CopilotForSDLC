using System.ComponentModel.DataAnnotations;
using LoanApplication.Api.Contracts;

namespace LoanApplication.Api.Tests;

public sealed class ContractValidationTests
{
    [Fact]
    public void CreateLoanApplicationRequest_WithValidValues_PassesValidation()
    {
        var request = new CreateLoanApplicationRequest
        {
            CustomerName = "Jane Doe",
            ExistingAccountId = "ACC-100200",
            LoanRequirement = "Home improvement loan"
        };

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void CreateLoanApplicationRequest_WithMissingValues_FailsValidation()
    {
        var request = new CreateLoanApplicationRequest();

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        Assert.False(isValid);
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(CreateLoanApplicationRequest.CustomerName)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(CreateLoanApplicationRequest.ExistingAccountId)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(CreateLoanApplicationRequest.LoanRequirement)));
    }

    [Fact]
    public void CreateLoanPrepaymentRequest_WithValidAmount_PassesValidation()
    {
        var request = new CreateLoanPrepaymentRequest
        {
            PrepaymentAmount = 250m
        };

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void CreateLoanPrepaymentRequest_WithZeroAmount_FailsValidation()
    {
        var request = new CreateLoanPrepaymentRequest();

        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        Assert.False(isValid);
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(CreateLoanPrepaymentRequest.PrepaymentAmount)));
    }
}
