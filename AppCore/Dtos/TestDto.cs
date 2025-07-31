using AppCore.BaseModel;

namespace AppCore.Dtos
{
    public class TestDto : BaseDto
    {
        /// <summary>
        /// Title of the test
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// Description of the test
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// List of questions in the test
        /// </summary>
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
        public PersonalityTypeDto? PersonalityType { get; set; }

        /// <summary>
        /// Identifier of the personality type associated with the test
        /// </summary>
        public Guid PersonalityTypeId { get; set; }
    }

    public class CreateTestDto
    {
        /// <summary>
        /// Title of the test
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// Description of the test
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// List of questions in the test
        /// </summary>
        public List<CreateQuestionDto> Questions { get; set; } = new List<CreateQuestionDto>();
        /// <summary>
        /// Identifier of the personality type associated with the test
        /// </summary>
        public Guid PersonalityTypeId { get; set; }
    }
}
