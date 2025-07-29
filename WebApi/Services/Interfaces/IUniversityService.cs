using AppCore.BaseModel;
using AppCore.Dtos;

namespace ApiService.Services.Interfaces
{
    public interface IUniversityService
    {
        Task<ApiResponse> GetUniversityByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetAllUniversitiesAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse> CreateUniversityAsync(CreateUpdateUniversityDto universityDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
        Task<ApiResponse> UpdateUniversityAsync(Guid id, CreateUpdateUniversityDto universityDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteUniversityAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
