using AppCore.BaseModel;

namespace AppCore.Dtos
{
    public class AnswerDto : BaseDto
    {
        public string Text { get; set; } = string.Empty;
        public Guid QuestionId { get; set; }
    }
}
