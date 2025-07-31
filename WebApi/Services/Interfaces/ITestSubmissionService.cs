using AppCore.BaseModel;
using AppCore.Dtos;

namespace ApiService.Services.Interfaces
{
    public interface ITestSubmissionService
    {
        Task<ApiResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetByPersonalTypeIdAsync(Guid personTypeId, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse> CreateAsync(TestSubmissionDto testSubmissionDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
    }
}
