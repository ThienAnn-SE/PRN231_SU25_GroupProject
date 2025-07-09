using AppCore.BaseModel;

namespace AppCore.Dtos
{
    public class QuestionDto : BaseDto
    {
        public string Text { get; set; } = string.Empty;
        public Guid TestId { get; set; }
        public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
    }
}
