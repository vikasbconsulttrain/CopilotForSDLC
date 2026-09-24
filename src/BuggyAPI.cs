```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();


// ============================================================
// BUG-INFESTED BANKING API
// ============================================================
// Intentionally insecure code for GitHub Copilot Secure Coding
// training.
//
// Endpoints:
// GET    /accounts
// GET    /accounts/{id}
// POST   /payments
// DELETE /beneficiaries/{id}
//
// DO NOT use this code in production.
// ============================================================


// ------------------------------------------------------------
// Fake data
// ------------------------------------------------------------

var accounts = new List<Account>
{
    new Account
    {
        Id = 1001,
        CustomerId = 501,
        AccountNumber = "XXXX-1001",
        Balance = 250000
    },
    new Account
    {
        Id = 1002,
        CustomerId = 502,
        AccountNumber = "XXXX-1002",
        Balance = 175000
    },
    new Account
    {
        Id = 1003,
        CustomerId = 503,
        AccountNumber = "XXXX-1003",
        Balance = 95000
    }
};

var beneficiaries = new List<Beneficiary>
{
    new Beneficiary
    {
        Id = 2001,
        CustomerId = 501,
        Name = "ABC Enterprises",
        AccountNumber = "9988776655"
    },
    new Beneficiary
    {
        Id = 2002,
        CustomerId = 502,
        Name = "XYZ Services",
        AccountNumber = "8877665544"
    }
};


// ------------------------------------------------------------
// GET /accounts
// ------------------------------------------------------------
// BUG #1:
// No authentication.
//
// Anyone can call this endpoint.
//
// BUG #2:
// Returns information belonging to ALL customers.
//
// BUG #3:
// Sensitive account information is exposed.
// ------------------------------------------------------------

app.MapGet("/accounts", () =>
{
    Console.WriteLine("Returning all customer accounts");

    return Results.Ok(accounts);
});


// ------------------------------------------------------------
// GET /accounts/{id}
// ------------------------------------------------------------
// BUG #4:
// No authentication.
//
// BUG #5:
// IDOR / Broken Object Level Authorization.
//
// A customer can simply change:
// /accounts/1001
// to
// /accounts/1002
//
// and retrieve another customer's account.
// ------------------------------------------------------------

app.MapGet("/accounts/{id}", (int id) =>
{
    var account = accounts.FirstOrDefault(x => x.Id == id);

    if (account == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(account);
});


// ------------------------------------------------------------
// POST /payments
// ------------------------------------------------------------
// BUG #6:
// No authentication.
//
// BUG #7:
// No authorization.
//
// BUG #8:
// No meaningful input validation.
//
// BUG #9:
// Allows arbitrary payment amount.
//
// BUG #10:
// Negative payment amount is possible.
//
// BUG #11:
// No account ownership validation.
//
// BUG #12:
// No beneficiary ownership validation.
//
// BUG #13:
// Sensitive payment details are logged.
//
// BUG #14:
// Hard-coded database credentials.
//
// BUG #15:
// SQL query constructed using string interpolation.
// SQL Injection vulnerability.
//
// BUG #16:
// No transaction handling.
//
// BUG #17:
// No duplicate payment protection.
//
// BUG #18:
// No rate limiting.
//
// BUG #19:
// No fraud/security checks.
// ------------------------------------------------------------

app.MapPost("/payments", (PaymentRequest request) =>
{
    Console.WriteLine(
        $"Processing payment: " +
        $"FromAccount={request.FromAccountId}, " +
        $"ToBeneficiary={request.BeneficiaryId}, " +
        $"Amount={request.Amount}, " +
        $"CardNumber={request.CardNumber}");

    // Hard-coded secret
    var connectionString =
        "Server=prod-db;Database=Banking;" +
        "User Id=admin;Password=SuperSecret123!;";

    // No validation of amount
    if (request.Amount == 0)
    {
        return Results.BadRequest("Invalid amount");
    }

    // BUG: SQL injection
    var sql =
        $"INSERT INTO Payments " +
        $"(FromAccountId, BeneficiaryId, Amount) " +
        $"VALUES ({request.FromAccountId}, " +
        $"{request.BeneficiaryId}, " +
        $"{request.Amount})";

    Console.WriteLine($"Executing SQL: {sql}");

    try
    {
        using var connection = new SqlConnection(connectionString);

        // Intentionally omitted:
        // connection.Open();
        // command.ExecuteNonQuery();

        // Simulating successful payment
        return Results.Ok(new
        {
            Status = "SUCCESS",
            Message = "Payment processed",
            Amount = request.Amount
        });
    }
    catch (Exception ex)
    {
        // BUG #20:
        // Exposes internal exception details.
        return Results.BadRequest(
            new
            {
                Error = ex.ToString()
            });
    }
});


// ------------------------------------------------------------
// DELETE /beneficiaries/{id}
// ------------------------------------------------------------
// BUG #21:
// No authentication.
//
// BUG #22:
// No authorization.
//
// BUG #23:
// IDOR.
//
// Any caller who knows a beneficiary ID can delete it.
//
// BUG #24:
// No confirmation / business rule.
//
// BUG #25:
// No audit logging.
//
// BUG #26:
// Errors expose internal information.
// ------------------------------------------------------------

app.MapDelete("/beneficiaries/{id}", (int id) =>
{
    var beneficiary =
        beneficiaries.FirstOrDefault(x => x.Id == id);

    if (beneficiary == null)
    {
        throw new Exception(
            $"Beneficiary {id} does not exist in the database.");
    }

    beneficiaries.Remove(beneficiary);

    Console.WriteLine(
        $"Deleted beneficiary: " +
        $"Id={beneficiary.Id}, " +
        $"CustomerId={beneficiary.CustomerId}, " +
        $"AccountNumber={beneficiary.AccountNumber}");

    return Results.Ok(
        new
        {
            Message = "Beneficiary deleted"
        });
});


// ------------------------------------------------------------
// Global error handler - INTENTIONALLY BAD
// ------------------------------------------------------------
// BUG #27:
// Returns internal exception information to callers.
// ------------------------------------------------------------

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;

        await context.Response.WriteAsJsonAsync(
            new
            {
                Error = ex.Message,
                StackTrace = ex.StackTrace
            });
    }
});


app.Run();


// ============================================================
// Models
// ============================================================

public class Account
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string AccountNumber { get; set; } = "";

    public decimal Balance { get; set; }
}


public class Beneficiary
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string Name { get; set; } = "";

    public string AccountNumber { get; set; } = "";
}


public class PaymentRequest
{
    public int FromAccountId { get; set; }

    public int BeneficiaryId { get; set; }

    public decimal Amount { get; set; }

    // Intentionally included to create
    // an additional sensitive-data handling problem.
    public string CardNumber { get; set; } = "";
}
```

