using AppCore.BaseModel;

namespace AppCore.Dtos
{
    public class PersonalityTypeDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
