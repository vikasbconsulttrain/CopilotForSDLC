using LoanApplicationEntity = LoanApplication.Api.Domain.LoanApplication;

namespace LoanApplication.Api.Repositories;

public interface ILoanApplicationRepository
{
    Task<LoanApplicationEntity> AddAsync(LoanApplicationEntity application, CancellationToken cancellationToken = default);

    Task<LoanApplicationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
