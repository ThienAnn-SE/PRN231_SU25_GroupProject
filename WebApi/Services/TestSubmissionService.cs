using AppCore.BaseModel;
using AppCore.Dtos;
using Repositories;
using System.Threading.Tasks;
using WebApi.Extension;

namespace WebApi.Services
{
    public interface ITestSubmissionService
    {
        Task<ApiResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse> CreateAsync(TestSubmissionDto testSubmissionDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
    }

    public class TestSubmissionService : BaseService, ITestSubmissionService
    {
        public TestSubmissionService(IUnitOfWork unitOfWork) : base(unitOfWork) 
        {
        }

        public async Task<ApiResponse> CreateAsync(TestSubmissionDto testSubmissionDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            // Validate the test submission DTO
            if (testSubmissionDto == null)
            {
                return ApiResponse.CreateBadRequestResponse("Test submission data cannot be null.");
            }
            if (testSubmissionDto.TestId == Guid.Empty || testSubmissionDto.ExamineeId == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("Test ID and Examinee ID cannot be empty.");
            }
            if (testSubmissionDto.Answers == null || !testSubmissionDto.Answers.Any())
            {
                return ApiResponse.CreateBadRequestResponse("Test submission must have at least one answer.");
            }
            var examinee = await unitOfWork.UserProfiles.GetProfileAsync(testSubmissionDto.ExamineeId, cancellationToken);
            if (examinee == null)
            {
                return ApiResponse.CreateNotFoundResponse("Examinee not found.");
            }
            var test = await unitOfWork.TestRepository.GetByIdAsync(testSubmissionDto.TestId, cancellationToken);
            if (test == null)
            {
                return ApiResponse.CreateNotFoundResponse("Test not found.");
            }
            var personalityType = await unitOfWork.PersonalityRepository.GetTypeByIdAsync(test.PersonalityTypeId, cancellationToken);
            if (personalityType == null)
            {
                return ApiResponse.CreateNotFoundResponse("Personality type not found.");
            }
            if (personalityType.Name == "MBTI" )
            {
                if (testSubmissionDto.Answers.Count != 50)
                {
                    return ApiResponse.CreateBadRequestResponse("MBTI tests must have exactly 50 answers.");
                }
                testSubmissionDto.PersonalityId = await CalculateMBTIResult(test, testSubmissionDto.Answers);
            }
            else if (personalityType.Name == "DISC")
            {
                if (testSubmissionDto.Answers.Count != 28)
                {
                    return ApiResponse.CreateBadRequestResponse("DISC tests must have exactly 28 answers.");
                }
                // Implement DISC result calculation here
                // testSubmissionDto.PersonalityId = await CalculateDISCResult(test, testSubmissionDto.Answers);
            }
            else
            {
                return ApiResponse.CreateBadRequestResponse("Unsupported personality type for test submission.");
            }
            await unitOfWork.TestSubmissionRepository.CreateAsync(testSubmissionDto, creatorId, cancellationToken);
            return ApiResponse.CreateSuccessResponse("Test submission created successfully.");
        }

        public Task<ApiResponse> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private async Task<Guid> CalculateMBTIResult(TestDto test, List<Guid> answers)
        {
            var userAnswers = new List<int>(answers.Count);
            for (int i = 0; i < test.Questions.Count; i++)
            {
                var question = test.Questions[i];
                var answerIndex = question.Answers.FindIndex(a => a.Id == answers[i]);
                if (answerIndex < 0 || answerIndex >= question.Answers.Count)
                {
                    throw new ArgumentException($"Invalid answer for question {i + 1}.");
                }
                userAnswers.Add(answerIndex);
            }
            var result = TestEvaluationExtensions.EvaluateMBTI(userAnswers);
            var personality = await unitOfWork.PersonalityRepository.GetByNameAsync(result, CancellationToken.None);
            if (personality == null)
            {
                throw new InvalidOperationException($"Personality type '{result}' not found in the database.");
            }
            return personality.Id;
        }
    }
}
