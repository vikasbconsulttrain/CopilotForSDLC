using System.Collections.Concurrent;
using LoanApplicationEntity = LoanApplication.Api.Domain.LoanApplication;

namespace LoanApplication.Api.Repositories;

public sealed class InMemoryLoanApplicationRepository : ILoanApplicationRepository
{
    private readonly ConcurrentDictionary<Guid, LoanApplicationEntity> _store = new();

    public Task<LoanApplicationEntity> AddAsync(LoanApplicationEntity application, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(application);

        _store[application.Id] = application;
        return Task.FromResult(application);
    }

    public Task<LoanApplicationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _store.TryGetValue(id, out var application);
        return Task.FromResult(application);
    }
}
