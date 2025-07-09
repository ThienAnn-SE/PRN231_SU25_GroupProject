using AppCore.BaseModel;

namespace AppCore.Dtos
{
    public class TestSubmissionDto : BaseDto
    {
        public DateTime Date { get; set; }
        public Guid TestId { get; set; }
        public Guid ExamineeId { get; set; }
        public Guid PersonalityId { get; set; }
        public List<Guid> Answers { get; set; } = new List<Guid>();
    }
}
