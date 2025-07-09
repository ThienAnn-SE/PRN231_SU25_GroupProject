using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ITestSubmissionRepository
    {
        Task<TestSubmissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<TestSubmissionDto>> GetAll(CancellationToken cancellationToken = default);

        Task<bool> CreateAsync(TestSubmissionDto testSubmissionDto, Guid? creatorId = null, CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(TestSubmissionDto testSubmissionDto, Guid? updaterId = null, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class TestSubmissionRepository : ITestSubmissionRepository
    {
        private readonly CrudRepository<TestSubmission> _testSubmissionRepository;
        private readonly CrudRepository<AnswerSubmission> _answerSubmissionRepository;

        public TestSubmissionRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _testSubmissionRepository = new CrudRepository<TestSubmission>(dbContext, transaction);
            _answerSubmissionRepository = new CrudRepository<AnswerSubmission>(dbContext, transaction);
        }

        public Task<bool> CreateAsync(TestSubmissionDto testSubmissionDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<TestSubmissionDto>> GetAll(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TestSubmissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(TestSubmissionDto testSubmissionDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
