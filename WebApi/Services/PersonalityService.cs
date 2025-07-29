using ApiService.Services.Interfaces;
using AppCore.BaseModel;
using AppCore.Dtos;
using Repositories.Interfaces;

namespace ApiService.Services
{
    public class PersonalityService : BaseService, IPersonalityService
    {
        public PersonalityService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public async Task<ApiResponses<PersonalityDetailDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var personalities =  await unitOfWork.PersonalityRepository.GetAll();
            if (personalities == null || !personalities.Any())
            {
                return ApiResponses<PersonalityDetailDto>.CreateNotFoundResponse(
                    default,
                    "No personalities found."
                );
            }

            return ApiResponses<PersonalityDetailDto>.CreateResponse(
                System.Net.HttpStatusCode.OK,
                true,
                "Personalities retrieved successfully.",
                personalities
            );
        }

        public async Task<ApiResponses<PersonalityTypeDto>> GetTypeAll(CancellationToken cancellationToken = default)
        {
            var personalityTypes = await unitOfWork.PersonalityRepository.GetTypeAll(cancellationToken);
            if (personalityTypes == null || !personalityTypes.Any())
            {
                return ApiResponses<PersonalityTypeDto>.CreateNotFoundResponse(
                    default,
                    "No personality types found."
                );
            }
            return ApiResponses<PersonalityTypeDto>.CreateResponse(
                System.Net.HttpStatusCode.OK,
                true,
                "Personality types retrieved successfully.",
                personalityTypes
            );
        }

        public async Task<ApiResponse<PersonalityDto>> GetById(Guid id, CancellationToken cancellationToken = default)
        {
            var personality = await unitOfWork.PersonalityRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (personality == null)
            {
                return ApiResponse<PersonalityDto>.CreateNotFoundResponse(
                    default,
                    "Personality not found."
                );
            }
            return ApiResponse<PersonalityDto>.CreateResponse(
                System.Net.HttpStatusCode.OK,
                true,
                "Personality retrieved successfully.",
                personality
            );
        }

        public async Task<ApiResponse<PersonalityTypeDto>> GetTypeById(Guid id, CancellationToken cancellationToken = default)
        {
            var personalityType = await unitOfWork.PersonalityRepository.GetTypeByIdAsync(id, cancellationToken: cancellationToken);
            if (personalityType == null)
            {
                return ApiResponse<PersonalityTypeDto>.CreateNotFoundResponse(
                    default,
                    "Personality type not found."
                );
            }
            return ApiResponse<PersonalityTypeDto>.CreateResponse(
                System.Net.HttpStatusCode.OK,
                true,
                "Personality type retrieved successfully.",
                personalityType
            );
        }
    }
}
