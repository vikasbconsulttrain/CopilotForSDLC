using LoanApplication.Api.Repositories;
using LoanApplication.Api.Orm;
using LoanApplication.Api.Orm.Services;
using LoanApplication.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ILoanApplicationRepository, InMemoryLoanApplicationRepository>();
builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();
builder.Services.AddDbContext<BankingDbContext>(options => options.UseInMemoryDatabase("LendingDb"));
builder.Services.AddScoped<ILendingQueryService, LendingQueryService>();
builder.Services.AddScoped<ILoanPrepaymentService, LoanPrepaymentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
