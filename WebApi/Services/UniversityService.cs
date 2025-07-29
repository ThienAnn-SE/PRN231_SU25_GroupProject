using ApiService.Services.Interfaces;
using AppCore.BaseModel;
using AppCore.Dtos;
using Repositories.Interfaces;
using WebApi.Extension;

namespace ApiService.Services
{
    public class UniversityService : BaseService, IUniversityService
    {

        public UniversityService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public async Task<ApiResponse> GetUniversityByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return ApiResponse.CreateBadRequestResponse("Invalid university ID provided."); // Return null if the ID is invalid
            }
            var university = await unitOfWork.UniversityRepository.GetByIdAsync(id, cancellationToken);
            if (university == null)
            {
                return ApiResponse.CreateNotFoundResponse("University does not exist"); // Return null if the university is not found
            }
            return ApiResponse<UniversityDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Retrived university successfully!", university);
        }

        public async Task<ApiResponse> GetAllUniversitiesAsync(CancellationToken cancellationToken = default)
        {
            var universities = await unitOfWork.UniversityRepository.GetAll(cancellationToken);
            if (universities == null || !universities.Any())
            {
                return ApiResponse.CreateNotFoundResponse("No universities found.");
            }
            return ApiResponses<UniversityDto>.CreateResponse(System.Net.HttpStatusCode.OK, true, "Retrived universities successfully", universities);
        }

        public async Task<ApiResponse> CreateUniversityAsync(CreateUpdateUniversityDto universityDto, Guid? creatorId = null, CancellationToken cancellationToken = default)
        {
            if (IsValidCreateUpdateUniversityDto(universityDto, out var apiResponse) == false)
            {
                return apiResponse; // Return the error response if validation fails
            }
            var success = await unitOfWork.UniversityRepository.CreateAsync(universityDto, creatorId, cancellationToken);
            if (!success)
            {
                return ApiResponse.CreateBadRequestResponse("University with this name might already exist or name is invalid.");
            }
            return ApiResponse.CreateSuccessResponse("Create new university successfully");
        }

        public async Task<ApiResponse> UpdateUniversityAsync(Guid id, CreateUpdateUniversityDto universityDto, Guid? updaterId = null, CancellationToken cancellationToken = default)
        {
            if (IsValidCreateUpdateUniversityDto(universityDto, out var apiResponse) == false)
            {
                return apiResponse; // Return the error response if validation fails
            }
            var result = await unitOfWork.UniversityRepository.UpdateAsync(id, universityDto, updaterId, cancellationToken);
            if (!result)
            {
                return ApiResponse.CreateNotFoundResponse("University does not exist or update failed.");
            }
            return ApiResponse.CreateSuccessResponse("Updated university successfully");
        }

        public async Task<bool> DeleteUniversityAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await unitOfWork.UniversityRepository.DeleteAsync(id, cancellationToken);
        }

        private bool IsValidCreateUpdateUniversityDto(CreateUpdateUniversityDto universityDto, out ApiResponse apiResponse)
        {
            if (string.IsNullOrWhiteSpace(universityDto.Name))
            {
                apiResponse = ApiResponse.CreateBadRequestResponse("University must contain name");
                return false;
            }
            if (universityDto.Name.Length < 3 || universityDto.Name.Length > 100)
            {
                apiResponse = ApiResponse.CreateBadRequestResponse("University name must be between 3 and 100 characters long.");
                return false;
            }
            if (universityDto.PhoneNumber != null && universityDto.PhoneNumber.Length < 10)
            {
                apiResponse = ApiResponse.CreateBadRequestResponse("Phone number must be at least 10 digits long.");
                return false;
            }
            if (universityDto.Email != null && !universityDto.Email.IsValidEmail())
            {
                apiResponse = ApiResponse.CreateBadRequestResponse("Invalid email format.");
                return false;
            }
            if (universityDto.Website != null && !Uri.IsWellFormedUriString(universityDto.Website, UriKind.Absolute))
            {
                apiResponse = ApiResponse.CreateBadRequestResponse("Invalid website URL.");
                return false;
            }
            if (universityDto.Description != null && universityDto.Description.Length > 2000)
            {
                apiResponse = ApiResponse.CreateBadRequestResponse("Description cannot exceed 2000 characters.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(universityDto.Location))
            {
                apiResponse = ApiResponse.CreateBadRequestResponse("Location must include a valid location.");
                return false;
            }
            apiResponse = ApiResponse.CreateSuccessResponse("Valid university data.");
            return true;
        }
    }
}
