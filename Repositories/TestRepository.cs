using System.Linq.Expressions;
using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ITestRepository
    {
        // Define methods for the TestRepository here, e.g.:
        Task<TestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<TestDto>> GetAll(CancellationToken cancellationToken = default);
        Task<bool> CreateAsync(TestDto testDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TestDto testDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class TestRepository : ITestRepository
    {
        private readonly CrudRepository<Test> _testRepository;
        private readonly CrudRepository<Question> _questionRepository;
        private readonly CrudRepository<Answer> _answerRepository;

        public TestRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _testRepository = new CrudRepository<Test>(dbContext, transaction);
            _questionRepository = new CrudRepository<Question>(dbContext, transaction);
            _answerRepository = new CrudRepository<Answer>(dbContext, transaction);
        }


        public Task<bool> CreateAsync(TestDto testDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Test, bool>>[]
            {
                x => x.Title == testDto.Title
            };
            var existingTest = _testRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingTest != null)
            {
                return Task.FromResult(false);
            }

            var newTest = new Test
            {
                Id = Guid.NewGuid(),
                Title = testDto.Title,
                Description = testDto.Description,
                CreatedAt = DateTime.UtcNow,

            };
            return _testRepository.SaveAsync(newTest, creatorId, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Test, bool>>[]
            {
                x => x.Id == id
            };
            var existingTest = _testRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingTest == null)
            {
                return false;
            }

            return await _testRepository.HardDeleteAsync(id, cancellationToken);
        }

        public Task<List<TestDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var tests = _testRepository.GetAllAsync(cancellationToken: cancellationToken);
            return tests.ContinueWith(t => t.Result.Select(test => new TestDto
            {
                Id = test.Id,
                Title = test.Title,
                Description = test.Description,
                CreatedAt = test.CreatedAt
            }).ToList(), cancellationToken);
        }

        public async Task<TestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Test, bool>>[]
            {
                x => x.Id == id
            };
            var existingTest = await _testRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingTest == null)
            {
                return null;
            }
            
            return new TestDto
            {
                Id = existingTest.Id,
                Title = existingTest.Title,
                Description = existingTest.Description,
                CreatedAt = existingTest.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(TestDto testDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Test, bool>>[]
            {
                x => x.Id == testDto.Id
            };
            var existingTest = await _testRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingTest == null)
            {
                return false;
            }

            var updatedTest = new Test
            {
                Id = existingTest.Id,
                Title = testDto.Title,
                Description = testDto.Description,
                CreatedAt = existingTest.CreatedAt
            };
            return await _testRepository.SaveAsync(updatedTest, updaterId, cancellationToken);
        }
    }
}
