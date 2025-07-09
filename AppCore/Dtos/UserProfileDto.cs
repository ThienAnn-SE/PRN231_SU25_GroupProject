using AppCore.BaseModel;
using AppCore.Entities;

namespace AppCore.Dtos
{
    public class UserProfileDto : BaseDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public DateTime DayOfBirth { get; set; }
        public Gender Gender { get; set; } = Gender.Other;
        public string Address { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public Guid UserAuthId { get; set; }
        public List<TestSubmissionDto> TestSubmissions { get; set; } = new List<TestSubmissionDto>();
    }
}
