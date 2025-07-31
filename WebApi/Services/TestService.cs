using AppCore.BaseModel;
using AppCore.Dtos;
using Repositories;
using WebApi.Extension;

namespace WebApi.Services
{
    public interface ITestService
    {
        // Define methods for the TestService here, e.g.:
        Task<ApiResponse> GetAllTestsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse> GetTestByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetTestByPersonalTypeAsync(Guid personalityTypeId, CancellationToken cancellationToken = default);
        Task<ApiResponse> CreateTestAsync(CreateTestDto testDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> UpdateTestAsync(TestDto testDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetTestByPersonalityTypeAsync(string typeName, CancellationToken cancellationToken = default);
    }

    public class TestService : BaseService, ITestService
    {
        public TestService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            // Initialize any dependencies or services here if needed
        }

        public async Task<ApiResponse> CreateTestAsync(CreateTestDto testDto, CancellationToken cancellationToken = default)
        {
                        if (string.IsNullOrWhiteSpace(testDto.Title) || string.IsNullOrWhiteSpace(testDto.Description))
            {
                return ApiResponse.CreateBadRequestResponse("Test title and description cannot be empty.");
            }
            if (testDto.Questions == null || !testDto.Questions.Any())
            {
                return ApiResponse.CreateBadRequestResponse("Test must have at least one question.");
            }
            foreach (var question in testDto.Questions)
            {
                if (string.IsNullOrWhiteSpace(question.Text) || question.Answers == null || !question.Answers.Any())
                {
                    return ApiResponse.CreateBadRequestResponse("Each question must have text and at least one answer.");
                }
                foreach (var answer in question.Answers)
                {
                    if (string.IsNullOrWhiteSpace(answer.Text))
                    {
                        return ApiResponse.CreateBadRequestResponse("Each answer must have text.");
                    }
                }
            }
            var personalityType = await unitOfWork.PersonalityRepository.GetTypeByIdAsync(testDto.PersonalityTypeId, cancellationToken);
            if (personalityType == null)
            {
                return ApiResponse.CreateNotFoundResponse("Personality type not found.");
            }
            switch (personalityType.Name)
            {
                case "MBTI":
                    if (testDto.Questions.Count != TestResource.MBTIQuestionCount)
                    {
                        return ApiResponse.CreateBadRequestResponse("MBTI tests must have 50 questions.");
                    }
                    if (!isValidMBTI(testDto))
                    {
                        return ApiResponse.CreateBadRequestResponse("Invalid MBTI test format");
                    }
                    break;
                case "OCEAN":
                    if (testDto.Questions.Count != TestResource.OCEANQuestionCount)
                    {
                        return ApiResponse.CreateBadRequestResponse("OCEAN tests must have 10 questions.");
                    }
                    if (!isValidOCEAN(testDto))
                    {
                        return ApiResponse.CreateBadRequestResponse("Invalid OCEAN test format");
                    }
                    break;
                case "DISC":
                    if (testDto.Questions.Count != TestResource.DISCQuestionCount)
                    {
                        return ApiResponse.CreateBadRequestResponse("DISC tests must have 28 questions.");
                    }
                    if (!isValidDISC(testDto))
                    {
                        return ApiResponse.CreateBadRequestResponse("Invalid DISC test format");
                    }
                    break;
                default:
                    return ApiResponse.CreateBadRequestResponse("Unsupported personality type for test creation.");
            }
            return await unitOfWork.TestRepository.CreateAsync(testDto, default, cancellationToken)
                ? ApiResponse.CreateResponse(System.Net.HttpStatusCode.Created, true, "Test created successfully.")
                : ApiResponse.CreateInternalServerErrorResponse("Failed to create test.");
        }

        private bool isValidMBTI(CreateTestDto testDto)
        {
            // MBTI tests should have exactly 50 questions
            foreach (var question in testDto.Questions)
            {
                // Each question have exactly 2 answers
                if (question.Answers.Count != 2)
                {
                    return false;
                }
            }
            return true;
        }

        private bool isValidOCEAN(CreateTestDto testDto)
        {
            // OCEAN tests should have exactly 10 questions
            foreach (var question in testDto.Questions)
            {
                // Each question should have exactly 5 answers
                if (question.Answers.Count != 5)
                {
                    return false;
                }
            }
            return true;
        }

        private bool isValidDISC(CreateTestDto testDto)
        {
            // DISC tests should have exactly 28 questions
            foreach (var question in testDto.Questions)
            {
                // Each question should have exactly 4 answers
                if (question.Answers.Count != 4)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<ApiResponse> GetAllTestsAsync(CancellationToken cancellationToken = default)
        {
            var tests = await unitOfWork.TestRepository.GetAll(cancellationToken);
            if (tests == null || !tests.Any())
            {
                return ApiResponse.CreateNotFoundResponse("No tests found.");
            }
            return ApiResponses<TestDto>.CreateResponse(
                System.Net.HttpStatusCode.OK,
                true,
                "Tests retrieved successfully.",
                tests
            );
        }

        public async Task<ApiResponse> GetTestByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
           var test = await unitOfWork.TestRepository.GetByIdAsync(id, cancellationToken);
            if (test == null)
            {
                return ApiResponse.CreateNotFoundResponse("Test not found.");
            }
            return ApiResponse<TestDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Test retrieved successfully.", test);
        }

        public Task<ApiResponse> GetTestByPersonalTypeAsync(Guid personalityTypeId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> UpdateTestAsync(TestDto testDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public async Task<ApiResponse> GetTestByPersonalityTypeAsync(string typeName, CancellationToken cancellationToken = default)
        {
            var allTests = await unitOfWork.TestRepository.GetAll(cancellationToken);

            var test = allTests.FirstOrDefault(t => t.PersonalityTypeId != Guid.Empty &&
                                                    string.Equals(t.PersonalityType?.Name, typeName, StringComparison.OrdinalIgnoreCase));

            if (test == null)
            {
                return ApiResponse.CreateNotFoundResponse("Test not found for the specified personality type.");
            }

            return ApiResponse<TestDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Test retrieved successfully.", test);
        }
    }
}
