using AppCore.BaseModel;

namespace AppCore.Dtos
{
    public class PersonalityDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid PersonalityTypeId { get; set; }
    }
}
