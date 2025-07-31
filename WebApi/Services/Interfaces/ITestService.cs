using AppCore.BaseModel;
using AppCore.Dtos;

namespace ApiService.Services.Interfaces
{
    public interface ITestService
    {
        // Define methods for the TestService here, e.g.:
        Task<ApiResponse> GetAllTestsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse> GetTestByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetRandomTest(Guid personalTypeId, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetTestByPersonalTypeAsync(Guid personalityTypeId, CancellationToken cancellationToken = default);
        Task<ApiResponse> CreateTestAsync(CreateTestDto testDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> UpdateTestAsync(TestDto testDto, CancellationToken cancellationToken = default);
    }
}
