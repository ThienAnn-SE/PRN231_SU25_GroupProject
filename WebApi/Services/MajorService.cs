using AppCore.Dtos;
using Repositories;

namespace WebApi.Services
{
    public interface IMajorService
    {
        Task<MajorDto?> GetMajorByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<MajorDto>> GetAllMajorsAsync(CancellationToken cancellationToken = default);
        Task<bool> CreateMajorAsync(CreateUpdateMajorDto majorDto, Guid? creatorId = null, CancellationToken cancellationToken = default);
        Task<bool> UpdateMajorAsync(Guid id, CreateUpdateMajorDto majorDto, Guid? updaterId = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteMajorAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class MajorService : IMajorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MajorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MajorDto?> GetMajorByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.MajorRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<List<MajorDto>> GetAllMajorsAsync(CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.MajorRepository.GetAll(cancellationToken);
        }

        public async Task<bool> CreateMajorAsync(CreateUpdateMajorDto majorDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(majorDto.Name) || string.IsNullOrWhiteSpace(majorDto.Description))
            {
                return false;
            }
            var success = await _unitOfWork.MajorRepository.CreateAsync(majorDto, creatorId, cancellationToken);
            return success;
        }

        public async Task<bool> UpdateMajorAsync(Guid id, CreateUpdateMajorDto majorDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(majorDto.Name) || string.IsNullOrWhiteSpace(majorDto.Description))
            {
                return false;
            }
            return await _unitOfWork.MajorRepository.UpdateAsync(id, majorDto, updaterId, cancellationToken);
        }

        public async Task<bool> DeleteMajorAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.MajorRepository.DeleteAsync(id, cancellationToken);
        }
    }
}
