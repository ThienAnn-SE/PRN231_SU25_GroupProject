using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
            var filter = new Expression<Func<University, bool>>[]
            {
                x => x.Name == universityDto.Name
            };
            var existingUniversity = _universityRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingUniversity != null)
            {
                return Task.FromResult(false);
            }

            var newUniversity = new University
            {
                Id = Guid.NewGuid(),
                Name = universityDto.Name,
                Location = universityDto.Location,
                PhoneNumber = universityDto.PhoneNumber,
                Email = universityDto.Email,
                Website = universityDto.Website,
                Description = universityDto.Description,
                CreatedAt = DateTime.UtcNow,

            };
            return _universityRepository.SaveAsync(newUniversity, creatorId, cancellationToken);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<University, bool>>[]
            {
                x => x.Id == id
            };
            var existingTest = _universityRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingTest == null)
            {
                return Task.FromResult(false);
            }

            return _universityRepository.HardDeleteAsync(id, cancellationToken);
        }

        public Task<List<UniversityDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var universities = _universityRepository.GetAllAsync(cancellationToken: cancellationToken);
            return universities.ContinueWith(t => t.Result.Select(university => new UniversityDto
            {
                Id = university.Id,
                Name = university.Name,
                Location = university.Location,
                PhoneNumber = university.PhoneNumber,
                Email = university.Email,
                Website = university.Website,
                Description = university.Description,
                CreatedAt = university.CreatedAt
            }).ToList(), cancellationToken);
        }

        public async Task<UniversityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<University, bool>>[]
            {
                x => x.Id == id
            };
            var existingUniversity = await _universityRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingUniversity == null)
            {
                return await Task.FromResult<UniversityDto?>(null);
            }

            return await Task.FromResult(new UniversityDto
            {
                Id = existingUniversity.Id,
                Name = existingUniversity.Name,
                Location = existingUniversity.Location,
                PhoneNumber = existingUniversity.PhoneNumber,
                Email = existingUniversity.Email,
                Website = existingUniversity.Website,
                Description = existingUniversity.Description,
                CreatedAt = existingUniversity.CreatedAt
            });
        }

        public async Task<bool> UpdateAsync(UniversityDto universityDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<University, bool>>[]
            {
                x => x.Id == universityDto.Id
            };
            var existingUniversity = await _universityRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingUniversity == null)
            {
                return await Task.FromResult(false);
            }

            var updatedUniversity = new University
            {
                Id = existingUniversity.Id,
                Name = universityDto.Name,
                Location = universityDto.Location,
                PhoneNumber = universityDto.PhoneNumber,
                Email = universityDto.Email,
                Website = universityDto.Website,
                Description = universityDto.Description,
                CreatedAt = existingUniversity.CreatedAt, 
                UpdatedAt = DateTime.UtcNow
            };
            return await _universityRepository.SaveAsync(updatedUniversity, updaterId, cancellationToken);
        }
    }
}
