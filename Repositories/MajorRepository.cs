using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System.Linq.Expressions;

namespace Repositories
{
    public class MajorRepository : IMajorRepository
    {
        private readonly CrudRepository<Major> _majorRepository;
        private readonly CrudRepository<MajorPersonality> _majorPersonalityRepository;

        private class MajorWithIncludesRepository : CrudRepository<Major>
        {
            public MajorWithIncludesRepository(DbContext dbContext, IDbTransaction transaction)
                : base(dbContext, transaction) { }

            protected override IQueryable<Major> IncludeProperties(DbSet<Major> dbSet)
            {
                return dbSet
                    .Include(m => m.University)
                    .Include(m => m.Personalities)
                        .ThenInclude(mp => mp.Personality);
            }
        }
        public MajorRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _majorRepository = new MajorWithIncludesRepository(dbContext, transaction);
            _majorPersonalityRepository = new CrudRepository<MajorPersonality>(dbContext, transaction);
        }

        public async Task<bool> CreateAsync(CreateUpdateMajorDto universityDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
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

        public async Task<List<MajorDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var majors = await _majorRepository.GetAllAsync(cancellationToken: cancellationToken);
            return majors.Select(university => new MajorDto
            {
                Id = university.Id,
                Name = university.Name,
                Description = university.Description,
                RequiredSkills = university.RequiredSkills,
                UniversityId = university.UniversityId,
                CreatedAt = university.CreatedAt,
                Personalities = university.Personalities.Select(mp => new PersonalityDto
                {
                    Id = mp.Personality.Id,
                    Name = mp.Personality.Name,
                    Description = mp.Personality.Description
                }).ToList()
            }).ToList();
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
                return null;
            }

            return new MajorDto
            {
                Id = existingMajor.Id,
                Name = existingMajor.Name,
                Description = existingMajor.Description,
                RequiredSkills = existingMajor.RequiredSkills,
                UniversityId = existingMajor.UniversityId,
                CreatedAt = existingMajor.CreatedAt,
                Personalities = existingMajor.Personalities.Select(mp => new PersonalityDto
                {
                    Id = mp.Personality.Id,
                    Name = mp.Personality.Name,
                    Description = mp.Personality.Description
                }).ToList()
            };
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

        public async Task<bool> AddNewPersonalityToMajor(MajorPersonalityDto majorPersonalityDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            var existingMajor = await _majorRepository.FindByIdAsync(majorPersonalityDto.MajorId, cancellationToken: CancellationToken.None);
            if (existingMajor == null)
            {
                return false;
            }
            var Personalities = new List<MajorPersonality>();
            foreach (var personalityId in majorPersonalityDto.PersonalityIds)
            {
                var filter = new Expression<Func<MajorPersonality, bool>>[]
                {
                    x => x.MajorId == majorPersonalityDto.MajorId && x.PersonalityId == personalityId
                };
                var existingMajorPersonality = await _majorPersonalityRepository.FindOneAsync(filter, cancellationToken: CancellationToken.None);

                if (existingMajorPersonality != null)
                {
                    continue; // Skip if this personality is already associated with the major
                }

                var newMajorPersonality = new MajorPersonality
                {
                    Id = Guid.NewGuid(),
                    MajorId = majorPersonalityDto.MajorId,
                    PersonalityId = personalityId,
                    CreatedAt = DateTime.UtcNow
                };
                Personalities.Add(newMajorPersonality);
            }
            if (Personalities.Count == 0 || !await _majorPersonalityRepository.SaveAllAsync(Personalities, null, CancellationToken.None))
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeletePersonalityFromMajor(MajorPersonalityDto majorPersonalityDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<MajorPersonality, bool>>[]
            {
                x => x.MajorId == majorPersonalityDto.MajorId && majorPersonalityDto.PersonalityIds.Contains(x.PersonalityId)
            };
            var existingMajorPersonality = await _majorPersonalityRepository.FindAsync(filter, cancellationToken: CancellationToken.None);
            if (existingMajorPersonality == null)
            {
                return false;
            }
            var deleteIds = existingMajorPersonality.Select(x => x.Id).ToList();
            if (deleteIds.Count == 0)
            {
                return false; // No personalities to delete
            }
            foreach (var id in deleteIds)
            {
                if (!await _majorPersonalityRepository.HardDeleteAsync(id, CancellationToken.None))
                {
                    return false; // If any deletion fails, return false
                }
            }
            return true; // All deletions were successful
        }

        public async Task<List<MajorDto>> GetMajorsByPersonalityIdAsync(Guid personalityId, CancellationToken cancellationToken = default)
        {
            if (personalityId == Guid.Empty)
            {
                return new List<MajorDto>();
            }
            var filter = new Expression<Func<Major, bool>>[]
            {
                x => x.Personalities.Any(mp => mp.PersonalityId == personalityId)
            };
            var majors = await _majorRepository.FindAsync(filter, cancellationToken: cancellationToken);

            if (majors == null || !majors.Any())
            {
                return new List<MajorDto>();
            }

            return majors.Select(major => new MajorDto
            {
                Id = major.Id,
                Name = major.Name,
                Description = major.Description,
                RequiredSkills = major.RequiredSkills,
                UniversityId = major.UniversityId,
                CreatedAt = major.CreatedAt,
                Personalities = major.Personalities.Select(mp => new PersonalityDto
                {
                    Id = mp.Personality.Id,
                    Name = mp.Personality.Name,
                    Description = mp.Personality.Description
                }).ToList()
            }).ToList();
        }
    }
}
