using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/loans/{loanId}", (int loanId) =>
{
    var loan = GetLoan(loanId);

    // Deliberate bug:
    // GetLoan() can return null, but we access CreditScore
    // without checking for null.
    var riskCategory = loan.CreditScore >= 700
        ? "LOW"
        : "HIGH";

    return Results.Ok(new
    {
        loan.LoanId,
        loan.CustomerName,
        loan.LoanAmount,
        loan.CreditScore,
        RiskCategory = riskCategory
    });
});

app.Run();

static Loan? GetLoan(int loanId)
{
    var loans = new List<Loan>
    {
        new Loan
        {
            LoanId = 1001,
            CustomerName = "John Smith",
            LoanAmount = 25000,
            CreditScore = 750
        },
        new Loan
        {
            LoanId = 1002,
            CustomerName = "Sarah Jones",
            LoanAmount = 50000,
            CreditScore = 680
        }
    };

    return loans.FirstOrDefault(x => x.LoanId == loanId);
}

public class Loan
{
    public int LoanId { get; set; }
    public string CustomerName { get; set; } = "";
    public decimal LoanAmount { get; set; }
    public int CreditScore { get; set; }
}
