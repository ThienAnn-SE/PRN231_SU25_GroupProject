using AppCore.Dtos;

namespace Repositories.Interfaces
{
    public interface ITestRepository
    {
        // Define methods for the TestRepository here, e.g.:
        Task<TestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<TestDto>> GetByPersonalTypeIdAsync(Guid personalTypeId, CancellationToken cancellationToken = default);
        Task<TestDto?> GetRandomTestAsync(Guid personalTypeId, CancellationToken cancellationToken = default);
        Task<List<TestDto>> GetAll(CancellationToken cancellationToken = default);
        Task<bool> CreateAsync(CreateTestDto testDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TestDto testDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
