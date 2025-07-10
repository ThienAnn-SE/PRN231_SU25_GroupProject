using AppCore.Dtos;
using Repositories;

namespace WebApi.Services
{
    public interface IUniversityService
    {
        Task<UniversityDto?> GetUniversityByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<UniversityDto>> GetAllUniversitiesAsync(CancellationToken cancellationToken = default);
        Task<bool> CreateUniversityAsync(UniversityDto universityDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
        Task<bool> UpdateUniversityAsync(UniversityDto universityDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteUniversityAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class UniversityService : IUniversityService
    {
        private readonly IUnitOfWork _unitOfWork; 

        public UniversityService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UniversityDto?> GetUniversityByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UniversityRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<List<UniversityDto>> GetAllUniversitiesAsync(CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UniversityRepository.GetAll(cancellationToken);
        }

        public async Task<bool> CreateUniversityAsync(UniversityDto universityDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(universityDto.Name))
            {
                return false;
            }

            var success = await _unitOfWork.UniversityRepository.CreateAsync(universityDto, creatorId, cancellationToken);
            
            return success;
        }

        public async Task<bool> UpdateUniversityAsync(UniversityDto universityDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(universityDto.Name))
            {
                return false;
            }
            return await _unitOfWork.UniversityRepository.UpdateAsync(universityDto, updaterId, cancellationToken);
        }

        public async Task<bool> DeleteUniversityAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UniversityRepository.DeleteAsync(id, cancellationToken);
        }
    }
}
