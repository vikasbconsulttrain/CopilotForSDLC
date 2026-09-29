using LoanApplication.Api.Contracts;
using LoanApplication.Api.Domain;
using LoanApplication.Api.Repositories;
using LoanApplicationEntity = LoanApplication.Api.Domain.LoanApplication;

namespace LoanApplication.Api.Services;

public sealed class LoanApplicationService : ILoanApplicationService
{
    private readonly ILoanApplicationRepository _repository;

    public LoanApplicationService(ILoanApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<LoanApplicationResponse> SubmitAsync(CreateLoanApplicationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var application = new LoanApplicationEntity
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName.Trim(),
            ExistingAccountId = request.ExistingAccountId.Trim(),
            LoanRequirement = request.LoanRequirement.Trim(),
            Status = LoanApplicationStatus.Submitted,
            SubmittedAtUtc = DateTimeOffset.UtcNow
        };

        var saved = await _repository.AddAsync(application, cancellationToken).ConfigureAwait(false);
        return Map(saved);
    }

    public async Task<LoanApplicationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var application = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return application is null ? null : Map(application);
    }

    private static LoanApplicationResponse Map(LoanApplicationEntity application)
    {
        return new LoanApplicationResponse
        {
            ApplicationId = application.Id,
            CustomerName = application.CustomerName,
            ExistingAccountId = application.ExistingAccountId,
            LoanRequirement = application.LoanRequirement,
            Status = application.Status.ToString(),
            SubmittedAtUtc = application.SubmittedAtUtc
        };
    }
}
