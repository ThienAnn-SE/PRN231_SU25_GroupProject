using AppCore.BaseModel;
using AppCore.Entities;

namespace AppCore.Dtos
{
    public class UniversityDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<MajorDto> Majors { get; set; } = new List<MajorDto>();
    }
}
