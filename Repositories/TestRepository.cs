using System.Linq.Expressions;
using AppCore;
using AppCore.Dtos;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public interface ITestRepository
    {
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

        public TestRepository(DbContext dbContext, IDbTransaction transaction)
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

            var test = new Test
            {
                Id = Guid.NewGuid(),
                Title = testDto.Title,
                Description = testDto.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PersonalityTypeId = testDto.PersonalityTypeId,
                CreatorId = creatorId
            };

            foreach (var questionDto in testDto.Questions)
            {
                var question = new Question
                {
                    Id = Guid.NewGuid(),
                    Text = questionDto.Text
                };

                foreach (var answerDto in questionDto.Answers)
                {
                    question.Answers.Add(new Answer
                    {
                        Id = Guid.NewGuid(),
                        Text = answerDto.Text
                    });
                }

                test.Questions.Add(question);
            }

            return await _testRepository.SaveAsync(test, creatorId, cancellationToken);
        }

        public async Task<List<TestDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var tests = await _testRepository.GetAllAsync(cancellationToken: cancellationToken);

            return tests.Select(test => new TestDto
            {
                Id = test.Id,
                PersonalityTypeId = test.PersonalityType.Id,
                PersonalityType = new PersonalityTypeDto
                {
                    Id = test.PersonalityType.Id,
                    Name = test.PersonalityType.Name,
                    Description = test.PersonalityType.Description
                },
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
                PersonalityType = new PersonalityTypeDto
                {
                    Id = existingTest.PersonalityType.Id,
                    Name = existingTest.PersonalityType.Name,
                    Description = existingTest.PersonalityType.Description
                },
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
            var existingTest = await _testRepository.FindByIdAsync(testDto.Id, cancellationToken);
            if (existingTest == null)
            {
                return false;
            }

            existingTest.Title = testDto.Title;
            existingTest.Description = testDto.Description;
            existingTest.UpdatedAt = DateTime.UtcNow;
            existingTest.EditorId = updaterId;

            // Optional: Update questions if needed
            // Hiện tại bạn chỉ cập nhật Test, không sửa câu hỏi

            return await _testRepository.UpdateAsync(existingTest, updaterId, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var test = await _testRepository.FindByIdAsync(id, cancellationToken);
            if (test == null)
            {
                return false;
            }

            return await _testRepository.HardDeleteAsync(id, cancellationToken);
        }
    }
}
