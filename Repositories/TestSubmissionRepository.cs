using System.Linq.Expressions;
using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories
{

    public class TestSubmissionRepository : ITestSubmissionRepository
    {
        private readonly CrudRepository<TestSubmission> _testSubmissionRepository;
        private class TestSubmissionWithIncludesRepository : CrudRepository<TestSubmission>
        {
            public TestSubmissionWithIncludesRepository(DbContext dbContext, IDbTransaction transaction)
                : base(dbContext, transaction) { }
            protected override IQueryable<TestSubmission> IncludeProperties(DbSet<TestSubmission> dbSet)
            {
                return dbSet
                    .Include(ts => ts.Examinee)
                    .Include(ts => ts.Personality)
                    .Include(ts => ts.Answers)
                        .ThenInclude(a => a.Answer);
            }
        }

        public TestSubmissionRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _testSubmissionRepository = new TestSubmissionWithIncludesRepository(dbContext, transaction);
        }

        public async Task<bool> CreateAsync(TestSubmissionDto testSubmissionDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            //var filter = new Expression<Func<TestSubmission, bool>>[]
            //{
            //    x => x.TestId == testSubmissionDto.TestId && x.ExamineeId == testSubmissionDto.ExamineeId
            //};
            //var existingSubmission = await _testSubmissionRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            //if (existingSubmission != null)
            //{
            //    return false;
            //}
            var newSubmission = new TestSubmission
            {
                Id = Guid.NewGuid(),
                Date = testSubmissionDto.Date,
                TestId = testSubmissionDto.TestId,
                ExamineeId = testSubmissionDto.ExamineeId,
                PersonalityId = testSubmissionDto.PersonalityId,
                Answers = testSubmissionDto.Answers.Select(a => new AnswerSubmission
                {
                    Id = Guid.NewGuid(),
                    AnswerId = a
                }).ToList(),
                CreatedAt = DateTime.UtcNow
            };
            return await _testSubmissionRepository.SaveAsync(newSubmission, creatorId, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<TestSubmission, bool>>[]
            {
                x => x.Id == id
            };
            var existingSubmission = await _testSubmissionRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingSubmission == null)
            {
                return false;
            }

            return await _testSubmissionRepository.HardDeleteAsync(id, cancellationToken);
        }

        public async Task<List<TestSubmissionDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var submissions = await _testSubmissionRepository.GetAllAsync(cancellationToken: cancellationToken);
            return submissions.Select(s => new TestSubmissionDto
            {
                Id = s.Id,
                Date = s.Date,
                TestId = s.TestId,
                ExamineeId = s.ExamineeId,
                PersonalityId = s.PersonalityId,
                Answers = s.Answers.Select(a => a.AnswerId).ToList()
            }).ToList();
        }

        public async Task<TestSubmissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<TestSubmission, bool>>[]
            {
                x => x.Id == id
            };
            var existingSubmission = await _testSubmissionRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingSubmission == null)
            {
                return null;
            }

            return new TestSubmissionDto
            {
                Id = existingSubmission.Id,
                Date = existingSubmission.Date,
                TestId = existingSubmission.TestId,
                ExamineeId = existingSubmission.ExamineeId,
                PersonalityId = existingSubmission.PersonalityId,
                Answers = existingSubmission.Answers.Select(a => a.AnswerId).ToList()
            };
        }

        public async Task<List<TestSubmissionDto>> GetByPersonalTypeId(Guid personalTypeId, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<TestSubmission, bool>>[]
{
                x => x.Test.PersonalityTypeId == personalTypeId
            };
            var existingSubmission = await _testSubmissionRepository.FindAsync(filter, cancellationToken: cancellationToken);
            if (existingSubmission != null)
            {
                var testSubmissionDtos = existingSubmission.Select(x => new TestSubmissionDto()
                {
                    Id = x.Id,
                    Date = x.Date,
                    TestId = x.TestId,
                    PersonalityId = x.PersonalityId,
                    EditorId = x.EditorId,
                    ExamineeId = x.ExamineeId,
                    CreatedAt = x.CreatedAt,
                    CreatorId = x.CreatorId,
                    UpdatedAt = x.UpdatedAt
                }).ToList();
                return testSubmissionDtos;
            }
            return [];
        }

        public async Task<bool> UpdateAsync(TestSubmissionDto testSubmissionDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<TestSubmission, bool>>[]
            {
                x => x.Id == testSubmissionDto.Id
            };
            var existingSubmission = await _testSubmissionRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingSubmission == null)
            {
                return false;
            }

            existingSubmission.Date = testSubmissionDto.Date;
            existingSubmission.TestId = testSubmissionDto.TestId;
            existingSubmission.ExamineeId = testSubmissionDto.ExamineeId;
            existingSubmission.PersonalityId = testSubmissionDto.PersonalityId;
            existingSubmission.Answers = testSubmissionDto.Answers.Select(a => new AnswerSubmission
            {
                Id = Guid.NewGuid(),
                AnswerId = a
            }).ToList();

            return await _testSubmissionRepository.SaveAsync(existingSubmission, updaterId, cancellationToken);
        }
    }
}
