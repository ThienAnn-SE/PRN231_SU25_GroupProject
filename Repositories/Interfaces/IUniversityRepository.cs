using AppCore.Dtos;

namespace Repositories.Interfaces
{
    public interface IUniversityRepository
    {
        Task<UniversityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<UniversityDto>> GetAll(CancellationToken cancellationToken = default);

        Task<bool> CreateAsync(CreateUpdateUniversityDto universityDto, Guid? creatorId = null, CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(Guid id, CreateUpdateUniversityDto dto, Guid? updaterId = null, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
