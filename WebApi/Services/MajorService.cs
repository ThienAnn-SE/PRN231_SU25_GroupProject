using AppCore.BaseModel;
using AppCore.Dtos;
using Repositories;

namespace WebApi.Services
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
        Task<ApiResponse> GetAllMajorsWithUniversityAsync(CancellationToken cancellationToken = default);
    }

    public class MajorService : IMajorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MajorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse> GetMajorByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("Major ID is required.");
            }
            var major = await _unitOfWork.MajorRepository.GetByIdAsync(id, cancellationToken);
            if (major == null)
            {
                return ApiResponse.CreateNotFoundResponse("No major found with the given ID.");
            }
            return ApiResponse<MajorDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Retrived major successfully", major);
        }

        public async Task<ApiResponse> GetAllMajorsAsync(CancellationToken cancellationToken = default)
        {
            var majors = await _unitOfWork.MajorRepository.GetAll(cancellationToken);
            if (majors == null || !majors.Any())
            {
                return ApiResponse.CreateNotFoundResponse("No majors found.");
            }
            return ApiResponses<MajorDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Retrieved all majors successfully", majors);
        }

        public async Task<ApiResponse> CreateMajorAsync(CreateUpdateMajorDto majorDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(majorDto.Name) || string.IsNullOrWhiteSpace(majorDto.Description))
            {
                return ApiResponse.CreateBadRequestResponse("Major name and description are required.");
            }
            var university = await _unitOfWork.UniversityRepository.GetByIdAsync(majorDto.UniversityId, cancellationToken);
            if (university == null)
            {
                return ApiResponse.CreateNotFoundResponse("University not found for the given ID.");
            }
            
            var success = await _unitOfWork.MajorRepository.CreateAsync(majorDto, creatorId, cancellationToken);
            if (!success)
            {
                return ApiResponse.CreateBadRequestResponse("Major with this name might already exist or name is invalid.");
            }
            return ApiResponse.CreateSuccessResponse("Create new major succesfully");
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

        public async Task<ApiResponse> AddNewPersonalityToMajorAsync(MajorPersonalityDto majorPersonalityDto, CancellationToken cancellationToken = default)
        {
            if (majorPersonalityDto.MajorId == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("Major ID is required to delete personality from major.");
            }
            if (majorPersonalityDto.PersonalityIds == null || !majorPersonalityDto.PersonalityIds.Any())
            {
                return ApiResponse.CreateBadRequestResponse("At least one personality ID is required to delete from major.");
            }
            if (majorPersonalityDto.PersonalityIds.Any(id => id == Guid.Empty))
            {
                return ApiResponse.CreateBadRequestResponse("All personality IDs must be valid GUIDs.");
            }
            if (majorPersonalityDto.PersonalityIds.Distinct().Count() != majorPersonalityDto.PersonalityIds.Count)
            {
                return ApiResponse.CreateBadRequestResponse("Duplicate personality IDs are not allowed.");
            }

            var major = await _unitOfWork.MajorRepository.GetByIdAsync(majorPersonalityDto.MajorId, cancellationToken);
            if (major == null)
            {
                return ApiResponse.CreateNotFoundResponse("Major not found.");
            }
            foreach (var personalityId in majorPersonalityDto.PersonalityIds)
            {
                var personality = await _unitOfWork.PersonalityRepository.GetByIdAsync(personalityId, cancellationToken);
                if (personality == null)
                {
                    return ApiResponse.CreateNotFoundResponse($"Personality with ID {personalityId} not found.");
                }
            }
            var success = await _unitOfWork.MajorRepository.AddNewPersonalityToMajor(majorPersonalityDto);
            if (!success)
            {
                return ApiResponse.CreateBadRequestResponse("Failed to add personality to major. Personality might already exist in this major.");
            }
            return ApiResponse.CreateSuccessResponse("Personality added to major successfully.");
        }

        public async Task<ApiResponse> DeletePersonalityFromMajorAsync(MajorPersonalityDto majorPersonalityDto, CancellationToken cancellationToken = default)
        {
            if (majorPersonalityDto.MajorId == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("Major ID is required to delete personality from major.");
            }
            if (majorPersonalityDto.PersonalityIds == null || !majorPersonalityDto.PersonalityIds.Any())
            {
                return ApiResponse.CreateBadRequestResponse("At least one personality ID is required to delete from major.");
            }
            if (majorPersonalityDto.PersonalityIds.Any(id => id == Guid.Empty))
            {
                return ApiResponse.CreateBadRequestResponse("All personality IDs must be valid GUIDs.");
            }
            if (majorPersonalityDto.PersonalityIds.Distinct().Count() != majorPersonalityDto.PersonalityIds.Count)
            {
                return ApiResponse.CreateBadRequestResponse("Duplicate personality IDs are not allowed.");
            }

            var major = await _unitOfWork.MajorRepository.GetByIdAsync(majorPersonalityDto.MajorId, cancellationToken);
            if (major == null)
            {
                return ApiResponse.CreateNotFoundResponse("Major not found.");
            }
            foreach (var personalityId in majorPersonalityDto.PersonalityIds)
            {
                var personality = await _unitOfWork.PersonalityRepository.GetByIdAsync(personalityId, cancellationToken);
                if (personality == null)
                {
                    return ApiResponse.CreateNotFoundResponse($"Personality with ID {personalityId} not found.");
                }
            }
            if (major.Personalities == null || !major.Personalities.Any(p => majorPersonalityDto.PersonalityIds.Contains(p.Id)))
            {
                return ApiResponse.CreateNotFoundResponse("No matching personalities found in the major.");
            }

            var success = await _unitOfWork.MajorRepository.DeletePersonalityFromMajor(majorPersonalityDto);

            if (!success)
            {
                return ApiResponse.CreateBadRequestResponse("Failed to delete personality from major. Personality might not exist in this major.");
            }
            return ApiResponse.CreateSuccessResponse("Personality deleted from major successfully.");
        }

        public async Task<ApiResponse> GetMajorsByPersonalityIdAsync(Guid personalityId, CancellationToken cancellationToken = default)
        {
            if (personalityId == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("Personality ID is required.");
            }
            var personality = await _unitOfWork.PersonalityRepository.GetByIdAsync(personalityId, cancellationToken);
            if (personality == null)
            {
                return ApiResponse.CreateNotFoundResponse("No personality found with the given ID.");
            }
            var majors = await _unitOfWork.MajorRepository.GetMajorsByPersonalityIdAsync(personalityId, cancellationToken);
            if (majors == null || !majors.Any())
            {
                return ApiResponse.CreateNotFoundResponse("No majors found for the given personality ID.");
            }
            return ApiResponses<MajorDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Retrieved majors successfully", majors);
        }
        public async Task<ApiResponse> GetAllMajorsWithUniversityAsync(CancellationToken cancellationToken = default)
        {
            var majors = await _unitOfWork.MajorRepository.GetAllWithUniversityAsync(cancellationToken);

            if (majors == null || !majors.Any())
            {
                return ApiResponse.CreateNotFoundResponse("No majors with university found.");
            }

            return ApiResponses<MajorWithUniversityDto>.CreateResponse(
                System.Net.HttpStatusCode.OK,
                true,
                "Retrieved majors with university successfully",
                majors
            );
        }
    }
}
