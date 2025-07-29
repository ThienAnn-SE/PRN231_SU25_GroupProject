using AppCore.Dtos;

namespace Repositories.Interfaces
{
    public interface IMajorRepository
    {
        Task<MajorDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<MajorDto>> GetMajorsByPersonalityIdAsync(Guid personalityId, CancellationToken cancellationToken = default);

        Task<List<MajorDto>> GetAll(CancellationToken cancellationToken = default);

        Task<bool> CreateAsync(CreateUpdateMajorDto universityDto, Guid? creatorId = null, CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(Guid id, CreateUpdateMajorDto dto, Guid? updaterId = null, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> AddNewPersonalityToMajor(MajorPersonalityDto majorPersonalityDto, Guid? updaterId = null, CancellationToken cancellationToken = default);

        Task<bool> DeletePersonalityFromMajor(MajorPersonalityDto majorPersonalityDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
    }
}
