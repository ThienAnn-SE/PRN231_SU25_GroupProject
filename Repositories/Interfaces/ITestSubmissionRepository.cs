using AppCore.Dtos;

namespace Repositories.Interfaces
{
    public interface ITestSubmissionRepository
    {
        Task<TestSubmissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<TestSubmissionDto>> GetAll(CancellationToken cancellationToken = default);

        Task<bool> CreateAsync(TestSubmissionDto testSubmissionDto, Guid? creatorId = null, CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(TestSubmissionDto testSubmissionDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        Task<List<TestSubmissionDto>> GetByPersonalTypeId(Guid personalTypeId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
