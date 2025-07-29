using AppCore.Dtos;

namespace Repositories.Interfaces
{
    public interface IPersonalityRepository
    {
        // Define methods for the PersonalityRepository here, e.g.:
        Task<PersonalityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PersonalityDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<PersonalityDto?> GetByNameAndTypeNameAsync(string name, string typeName, CancellationToken cancellationToken = default);
        Task<PersonalityTypeDto?> GetTypeByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<PersonalityDetailDto>> GetAll(CancellationToken cancellationToken = default);
        Task<List<PersonalityTypeDto>> GetTypeAll(CancellationToken cancellationToken = default);
        Task<bool> CreateAsync(PersonalityDto personalityDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(PersonalityDto personalityDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
