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
        Task<bool> CreateAsync(CreateTestDto testDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TestDto testDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class TestRepository : ITestRepository
    {
        private readonly CrudRepository<Test> _testRepository;

        private class TestWithIncludesRepository : CrudRepository<Test>
        {
            public TestWithIncludesRepository(DbContext dbContext, IDbTransaction transaction)
                : base(dbContext, transaction) { }

            protected override IQueryable<Test> IncludeProperties(DbSet<Test> dbSet)
            {
                return dbSet
                    .Include(t => t.PersonalityType)
                    .Include(t => t.Questions)
                        .ThenInclude(q => q.Answers);
            }
        }

        public TestRepository(
            DbContext dbContext,
            IDbTransaction transaction)
        {
            _testRepository = new TestWithIncludesRepository(dbContext, transaction);
        }

        public async Task<bool> CreateAsync(CreateTestDto testDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            var filter = new Expression<Func<Test, bool>>[]
            {
                x => x.Title == testDto.Title
            };
            var existingTest = await _testRepository.FindOneAsync(filter, cancellationToken: cancellationToken);
            if (existingTest != null)
            {
                return false;
            }
            var test = new Test();
            test.Id = Guid.NewGuid(); // Assuming you want to generate a new ID for the test
            test.Title = testDto.Title;
            test.Description = testDto.Description;
            test.CreatedAt = DateTime.UtcNow; // Assuming you want to set the creation time to now
            test.UpdatedAt = DateTime.UtcNow;
            test.PersonalityTypeId = testDto.PersonalityTypeId; // Assuming you have a PersonalityTypeId in TestDto
            test.CreatorId = creatorId;
            foreach (var questionDto in testDto.Questions)
            {
                var question = new Question();
                question.Text = questionDto.Text;
                foreach (var answerDto in questionDto.Answers)
                {
                    var answer = new Answer();
                    answer.Text = answerDto.Text;
                    question.Answers.Add(answer);
                }
                test.Questions.Add(question);
            }
            return await _testRepository.SaveAsync(test, creatorId, cancellationToken);
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

        public async Task<List<TestDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var tests = await _testRepository.GetAllAsync(cancellationToken: cancellationToken);
            var testDtos = tests.Select(test => new TestDto
            {
                Id = test.Id,
                PersonalityTypeId = test.PersonalityType.Id,
                Title = test.Title,
                Description = test.Description,
                CreatedAt = test.CreatedAt,
                Questions = test.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    TestId = test.Id,
                    Text = q.Text,
                    Answers = q.Answers.Select(a => new AnswerDto
                    {
                        Id = a.Id,
                        QuestionId = q.Id,
                        Text = a.Text
                    }).ToList()
                }).ToList()
            }).ToList();
            return testDtos;
        }

        public async Task<TestDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existingTest = await _testRepository.FindByIdAsync(id, cancellationToken: cancellationToken);
            if (existingTest == null)
            {
                return null;
            }
            
            return new TestDto
            {
                Id = existingTest.Id,
                PersonalityTypeId = existingTest.PersonalityType.Id,
                Title = existingTest.Title,
                Description = existingTest.Description,
                CreatedAt = existingTest.CreatedAt,
                Questions = existingTest.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    TestId = existingTest.Id,
                    Text = q.Text,
                    Answers = q.Answers.Select(a => new AnswerDto
                    {
                        Id = a.Id,
                        QuestionId = q.Id,
                        Text = a.Text
                    }).ToList()
                }).ToList()
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
                Questions = testDto.Questions.Select(q => new Question
                {
                    Id = q.Id,
                    Text = q.Text,
                    Answers = q.Answers.Select(a => new Answer
                    {
                        Id = a.Id,
                        Text = a.Text
                    }).ToList()
                }).ToList(),
                CreatedAt = existingTest.CreatedAt
            };
            return await _testRepository.SaveAsync(updatedTest, updaterId, cancellationToken);
        }
    }
}
