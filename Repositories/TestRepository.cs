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
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<TestDto>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(TestDto testDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
