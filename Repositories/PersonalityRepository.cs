using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface IPersonalityRepository
    {
        // Define methods for the PersonalityRepository here, e.g.:
        Task<PersonalityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PersonalityTypeDto?> GetTypeByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<PersonalityDto>> GetAll(CancellationToken cancellationToken = default);
        Task<List<PersonalityTypeDto>> GetTypeAll(CancellationToken cancellationToken = default);
        Task<bool> CreateAsync(PersonalityDto personalityDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(PersonalityDto personalityDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class PersonalityRepository : IPersonalityRepository
    {
        private readonly CrudRepository<Personality> _personalityRepository;
        private readonly CrudRepository<PersonalityType> _personalityTypeRepository;

        public PersonalityRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _personalityRepository = new CrudRepository<Personality>(dbContext, transaction);
            _personalityTypeRepository = new CrudRepository<PersonalityType>(dbContext, transaction);
        }
        public Task<bool> CreateAsync(PersonalityDto personalityDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<PersonalityDto>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PersonalityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<PersonalityTypeDto>> GetTypeAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PersonalityTypeDto?> GetTypeByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(PersonalityDto personalityDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
