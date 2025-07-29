using AppCore.BaseModel;
using AppCore.Dtos;

namespace ApiService.Services.Interfaces
{
    public interface IPersonalityService
    {
        Task<ApiResponses<PersonalityDetailDto>> GetAll(CancellationToken cancellationToken = default);
        Task<ApiResponses<PersonalityTypeDto>> GetTypeAll(CancellationToken cancellationToken = default);
        Task<ApiResponse<PersonalityDto>> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse<PersonalityTypeDto>> GetTypeById(Guid id, CancellationToken cancellationToken = default);

        //Task<ApiResponse> CreateAsync(PersonalityDto personalityDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
        //Task<ApiResponse> UpdateAsync(PersonalityDto personalityDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        //Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
