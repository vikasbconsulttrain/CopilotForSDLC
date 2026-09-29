using LoanApplication.Api.Contracts;

namespace LoanApplication.Api.Services;

public interface ILoanApplicationService
{
    Task<LoanApplicationResponse> SubmitAsync(CreateLoanApplicationRequest request, CancellationToken cancellationToken = default);

    Task<LoanApplicationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
