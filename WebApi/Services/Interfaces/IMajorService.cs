using AppCore.BaseModel;
using AppCore.Dtos;

namespace ApiService.Services.Interfaces
{
    public interface IMajorService
    {
        Task<ApiResponse> GetMajorByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetAllMajorsAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse> GetMajorsByPersonalityIdAsync(Guid personalityId, CancellationToken cancellationToken = default);
        Task<ApiResponse> CreateMajorAsync(CreateUpdateMajorDto majorDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
        Task<bool> UpdateMajorAsync(Guid id, CreateUpdateMajorDto majorDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteMajorAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse> AddNewPersonalityToMajorAsync(MajorPersonalityDto majorPersonalityDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> DeletePersonalityFromMajorAsync(MajorPersonalityDto majorPersonalityDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetRecommendMajorsAsync(Guid personalityId, CancellationToken cancellationToken = default);

    }
}
