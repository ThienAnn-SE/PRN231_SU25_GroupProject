using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface IUniversityRepository
    {
        Task<UniversityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<UniversityDto>> GetAll(CancellationToken cancellationToken = default);

        Task<bool> CreateAsync(UniversityDto universityDto, Guid? creatorId = null, CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(UniversityDto universityDto, Guid? updaterId = null, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class UniversityRepository : IUniversityRepository
    {
        private readonly CrudRepository<University> _universityRepository;
        private readonly CrudRepository<Major> _majorRepository;

        public UniversityRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _universityRepository = new CrudRepository<University>(dbContext, transaction);
            _majorRepository = new CrudRepository<Major>(dbContext, transaction);
        }

        public Task<bool> CreateAsync(UniversityDto universityDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<UniversityDto>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UniversityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(UniversityDto universityDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
