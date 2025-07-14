using AppCore.BaseModel;
using AppCore.Entities;

namespace AppCore.Dtos
{
    public class UserProfileDto : CreateUserProfileDto
    {
        public string FullName => $"{FirstName} {LastName}".Trim();
        public List<TestSubmissionDto> TestSubmissions { get; set; } = new List<TestSubmissionDto>();
    }

    public class CreateUserProfileDto : BaseDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DayOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public Guid UserAuthId { get; set; } = Guid.Empty;
    }
}
