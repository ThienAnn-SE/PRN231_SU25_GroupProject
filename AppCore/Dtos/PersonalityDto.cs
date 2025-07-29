using AppCore.BaseModel;

namespace AppCore.Dtos
{
    public class PersonalityDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid PersonalityTypeId { get; set; }
    }

    public class PersonalityDetailDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PersonalityTypeDto PersonalityType { get; set; } = new PersonalityTypeDto();
    }
    public class CreateUpdatePersonalityDto : BaseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid PersonalityTypeId { get; set; }
    }
}
