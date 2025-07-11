using AppCore.BaseModel;
using AppCore.Dtos;
using Repositories;

namespace WebApi.Services
{
    public interface IUniveristyService
    {
        // Define any additional methods specific to University service here
        Task<ApiResponse> GetUniversityByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetAllUniversitiesAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse> CreateUniversityAsync(UniversityDto universityDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> UpdateUniversityAsync(UniversityDto universityDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> DeleteUniversityAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse> GetAllMajors(CancellationToken cancellationToken = default);
        Task<ApiResponse> GetMajorById(Guid id, CancellationToken cancellationToken = default);
        Task<ApiResponse> CreateMajorAsync(MajorDto majorDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> UpdateMajorByIdAsync(MajorDto majorDto, CancellationToken cancellationToken = default);
        Task<ApiResponse> DeleteMajorByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
    public class UniveristyService : BaseService, IUniveristyService
    {
        public UniveristyService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Task<ApiResponse> CreateMajorAsync(MajorDto majorDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> CreateUniversityAsync(UniversityDto universityDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> DeleteMajorByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> DeleteUniversityAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetAllMajors(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetAllUniversitiesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetMajorById(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> GetUniversityByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> UpdateMajorByIdAsync(MajorDto majorDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> UpdateUniversityAsync(UniversityDto universityDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
