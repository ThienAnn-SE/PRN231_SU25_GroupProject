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
        /// <summary>
        /// Identifier of the personality type associated with the test
        /// </summary>
        public Guid PersonalityTypeId { get; set; }
    }
}
