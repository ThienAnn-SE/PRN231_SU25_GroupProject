using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System.Linq.Expressions;

namespace Repositories
{

    public class PersonalityRepository : IPersonalityRepository
    {
        //private readonly CrudRepository<Personality> _personalityRepository;
        private readonly CrudRepository<PersonalityType> _personalityTypeRepository;
        private readonly PersonalityWithIncludesRepository _personalityRepository;

        private class PersonalityWithIncludesRepository : CrudRepository<Personality>
        {
            public PersonalityWithIncludesRepository(DbContext dbContext, IDbTransaction transaction)
                : base(dbContext, transaction) { }
            protected override IQueryable<Personality> IncludeProperties(DbSet<Personality> dbSet)
            {
                return dbSet
                    .Include(p => p.PersonalityType);
            }
        }

        public PersonalityRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _personalityRepository = new PersonalityWithIncludesRepository(dbContext, transaction);
            _personalityTypeRepository = new CrudRepository<PersonalityType>(dbContext, transaction);
        }

        public async Task<bool> CreateAsync(PersonalityDto dto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            var entity = new Personality
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                PersonalityTypeId = dto.PersonalityTypeId,
            };

            await _personalityRepository.SaveAsync(entity, creatorId, cancellationToken);
            return true;
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PersonalityDetailDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Personality, bool>>[]
            {
                x => x.DeletedAt != default(DateTime) // Assuming DeletedAt is a DateTime field indicating soft deletion
            };
            var personalities = await _personalityRepository.FindAsync(filter, cancellationToken: cancellationToken);
            return personalities.Select(p => new PersonalityDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                PersonalityType = new PersonalityTypeDto
                {
                    Id = p.PersonalityTypeId,
                    Name = p.PersonalityType?.Name ?? string.Empty,
                    Description = p.PersonalityType?.Description ?? string.Empty
                },
            }).ToList();
        }

        public async Task<PersonalityDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Personality, bool>>[]
            {
                x => x.Id == id && x.DeletedAt == null // Assuming DeletedAt is a DateTime field indicating soft deletion
            };
            var personality = await _personalityRepository.FindOneAsync(filter, cancellationToken: cancellationToken);

            if (personality == null)
            {
                return null;
            }

            return new PersonalityDto
            {
                Id = personality.Id,
                Name = personality.Name,
                Description = personality.Description,
                PersonalityTypeId = personality.PersonalityTypeId
            };
        }

        public async Task<PersonalityDto?> GetByNameAndTypeNameAsync(string name, string typeName, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Personality, bool>>[]
            {
                x => x.Name == name && x.PersonalityType.Name == typeName && !x.DeletedAt.HasValue
            };
            var personality = await _personalityRepository.FindOneAsync(filter, cancellationToken: cancellationToken);

            if (personality == null)
            {
                return null;
            }

            return new PersonalityDto
            {
                Id = personality.Id,
                Name = personality.Name,
                Description = personality.Description,
                PersonalityTypeId = personality.PersonalityTypeId
            };
        }

        public async Task<PersonalityDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
             var filter = new Expression<Func<Personality, bool>>[]
            {
                x => x.Name == name && !x.DeletedAt.HasValue // Assuming DeletedAt is a DateTime field indicating soft deletion
            };
            var personality = await _personalityRepository.FindOneAsync(filter, cancellationToken: cancellationToken);

            if (personality == null)
            {
                return null;
            }

            return new PersonalityDto
            {
                Id = personality.Id,
                Name = personality.Name,
                Description = personality.Description,
                PersonalityTypeId = personality.PersonalityTypeId
            };
        }

        public async Task<List<PersonalityTypeDto>> GetTypeAll(CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<PersonalityType, bool>>[]
            {
                x => x.DeletedAt == null // Assuming DeletedAt is a DateTime field indicating soft deletion
            };
            var personalityTypes = await _personalityTypeRepository.FindAsync(filter, cancellationToken: cancellationToken);
            return personalityTypes.Select(pt => new PersonalityTypeDto
            {
                Id = pt.Id,
                Name = pt.Name,
                Description = pt.Description
            }).ToList();
        }

        public async Task<PersonalityTypeDto?> GetTypeByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<PersonalityType, bool>>[]
            {
                x => x.Id == id && x.DeletedAt == null // Assuming DeletedAt is a DateTime field indicating soft deletion
            };

            var personalityType = await _personalityTypeRepository.FindOneAsync(filter, cancellationToken: cancellationToken);

            if (personalityType == null)
            {
                return null;
            }

            return new PersonalityTypeDto
            {
                Id = personalityType.Id,
                Name = personalityType.Name,
                Description = personalityType.Description
            };
        }

        public Task<bool> UpdateAsync(PersonalityDto personalityDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
