using AppCore.BaseModel;
using AppCore.Dtos;
using Repositories;
using WebApi.Extension;

namespace WebApi.Services
{
    public interface ITestSubmissionService
    {
        Task<ApiResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetByPersonalTypeIdAsync(Guid personTypeId, CancellationToken cancellationToken = default);
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
            if (personalityType.Name == "MBTI")
            {
                if (testSubmissionDto.Answers.Count != TestResource.MBTIQuestionCount)
                {
                    return ApiResponse.CreateBadRequestResponse("MBTI tests must have exactly 50 answers.");
                }
                testSubmissionDto.PersonalityId = await CalculateMBTIResult(test, testSubmissionDto.Answers);
            }
            else if (personalityType.Name == "OCEAN")
            {
                if (testSubmissionDto.Answers.Count != TestResource.OCEANQuestionCount)
                {
                    return ApiResponse.CreateBadRequestResponse("OCEAN tests must have exactly 10 answers.");
                }
                testSubmissionDto.PersonalityId = await CalculateOCEANResult(test, testSubmissionDto.Answers);
            }
            else if (personalityType.Name == "DISC")
            {
                if (testSubmissionDto.Answers.Count != TestResource.DISCQuestionCount)
                {
                    return ApiResponse.CreateBadRequestResponse("DISC tests must have exactly 28 answers.");
                }
                // Implement DISC result calculation here
                testSubmissionDto.PersonalityId = await CalculateDISCResult(test, testSubmissionDto.Answers);
            }
            else
            {
                return ApiResponse.CreateBadRequestResponse("Unsupported personality type for test submission.");
            }
            await unitOfWork.TestSubmissionRepository.CreateAsync(testSubmissionDto, creatorId, cancellationToken);
            return ApiResponse.CreateSuccessResponse("Test submission created successfully.");
        }

        public async Task<ApiResponse> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var submissions = await unitOfWork.TestSubmissionRepository.GetAll(cancellationToken);
            if (submissions == null || !submissions.Any())
            {
                return ApiResponse.CreateNotFoundResponse("No test submissions found.");
            }
            return ApiResponses<TestSubmissionDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Retrieved test submissions successfully.", submissions);
        }

        public async Task<ApiResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("Test submission ID is required.");
            }
            var submission = await unitOfWork.TestSubmissionRepository.GetByIdAsync(id, cancellationToken);
            if (submission == null)
            {
                return ApiResponse.CreateNotFoundResponse("Test submission not found.");
            }
            return ApiResponse<TestSubmissionDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Retrived Testsubmission successfuly", submission);
        }

        public async Task<ApiResponse> GetByPersonalTypeIdAsync(Guid personTypeId, CancellationToken cancellationToken = default)
        {
            var personalType = await unitOfWork.PersonalityRepository.GetTypeByIdAsync(personTypeId, cancellationToken);
            if (personalType == null)
            {
                return ApiResponse.CreateNotFoundResponse($"Does not found personal type with ID {personTypeId}");
            }
            var testSubmissions = await unitOfWork.TestSubmissionRepository.GetByPersonalTypeId(personTypeId, cancellationToken);
            if (testSubmissions.Count == 0)
            {
                return ApiResponse.CreateNotFoundResponse($"Does not found any test submission with this ID: {personTypeId}");
            }
            return ApiResponses<TestSubmissionDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Retrived test submissions successfully!", testSubmissions);
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
            var personality = await unitOfWork.PersonalityRepository.GetByNameAndTypeNameAsync(result, "MBTI", CancellationToken.None);
            if (personality == null)
            {
                throw new InvalidOperationException($"Personality type '{result}' not found in the database.");
            }
            return personality.Id;
        }

        private async Task<Guid> CalculateOCEANResult(TestDto test, List<Guid> answers)
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
            var result = TestEvaluationExtensions.EvaluateOceanTest(userAnswers);
            var personality = await unitOfWork.PersonalityRepository.GetByNameAndTypeNameAsync(result, "OCEAN", CancellationToken.None);
            if (personality == null)
            {
                throw new InvalidOperationException($"Personality type '{result}' not found in the database.");
            }
            return personality.Id;
        }

        private async Task<Guid> CalculateDISCResult(TestDto test, List<Guid> answers)
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
            var result = TestEvaluationExtensions.EvaluateDiscTest(userAnswers);
            var personality = await unitOfWork.PersonalityRepository.GetByNameAndTypeNameAsync(result, "DISC", CancellationToken.None);
            if (personality == null)
            {
                throw new InvalidOperationException($"Personality type '{result}' not found in the database.");
            }
            return personality.Id;
        }
    }
}
