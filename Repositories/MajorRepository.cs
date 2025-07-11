using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IMajorRepository
    {
        Task<MajorDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<MajorDto>> GetAll(CancellationToken cancellationToken = default);

        Task<bool> CreateAsync(CreateUpdateMajorDto universityDto, Guid? creatorId = null, CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(Guid id, CreateUpdateMajorDto dto, Guid? updaterId = null, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class MajorRepository : IMajorRepository
    {
        private readonly CrudRepository<Major> _majorRepository;

        public MajorRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _majorRepository = new CrudRepository<Major>(dbContext, transaction);
        }

        public async Task<bool> CreateAsync(CreateUpdateMajorDto universityDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            //var filter = new Expression<Func<Major, bool>>[]
            //{
            //    x => x.Name == universityDto.Name
            //};
            //var existingMajor = await _majorRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            //if (existingMajor != null)
            //{
            //    return false;
            //}

            var newMajor = new Major
            {
                Id = Guid.NewGuid(),
                Name = universityDto.Name,
                Description = universityDto.Description,
                RequiredSkills = universityDto.RequiredSkills,
                UniversityId = universityDto.UniversityId,
                CreatedAt = DateTime.UtcNow,

            };
            return await _majorRepository.SaveAsync(newMajor, creatorId, cancellationToken);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Major, bool>>[]
            {
                x => x.Id == id
            };
            var existingTest = _majorRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingTest == null)
            {
                return Task.FromResult(false);
            }

            return _majorRepository.HardDeleteAsync(id, cancellationToken);
        }

        public Task<List<MajorDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var majors = _majorRepository.GetAllAsync(cancellationToken: cancellationToken);
            return majors.ContinueWith(t => t.Result.Select(university => new MajorDto
            {
                Id = university.Id,
                Name = university.Name,
                Description = university.Description,
                RequiredSkills = university.RequiredSkills,
                UniversityId = university.UniversityId,
                CreatedAt = university.CreatedAt
            }).ToList(), cancellationToken);
        }

        public async Task<MajorDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Major, bool>>[]
            {
                x => x.Id == id
            };
            var existingMajor = await _majorRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingMajor == null)
            {
                return await Task.FromResult<MajorDto?>(null);
            }

            return await Task.FromResult(new MajorDto
            {
                Id = existingMajor.Id,
                Name = existingMajor.Name,
                Description = existingMajor.Description,
                RequiredSkills = existingMajor.RequiredSkills,
                UniversityId = existingMajor.UniversityId,
                CreatedAt = existingMajor.CreatedAt
            });
        }

        public async Task<bool> UpdateAsync(Guid id, CreateUpdateMajorDto dto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Major, bool>>[]
            {
                x => x.Id == id
            };
            var existingMajor = await _majorRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingMajor == null)
            {
                return false;
            }

            existingMajor.Name = dto.Name;
            existingMajor.Description = dto.Description;
            existingMajor.RequiredSkills = dto.RequiredSkills;
            existingMajor.UpdatedAt = DateTime.UtcNow;
            return await _majorRepository.UpdateAsync(existingMajor, updaterId, cancellationToken);
        }
    }
}
