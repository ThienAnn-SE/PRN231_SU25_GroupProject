using AppCore.BaseModel;

namespace AppCore.Dtos
{
    public class MajorDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequiredSkills { get; set; } = string.Empty;
        public Guid UniversityId { get; set; }
        public List<PersonalityDto> Personalities { get; set; } = new List<PersonalityDto>();
    }
}
