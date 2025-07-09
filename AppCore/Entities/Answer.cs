using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class Answer : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public Guid QuestionId { get; set; }
        public Question? Question { get; set; } = null!; // Navigation property to the Question entity
        public List<AnswerSubmission> AnswerSubmissions { get; set; } = new List<AnswerSubmission>();
    }

    public class AnswerConfig : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.Property(x => x.Text)
                .IsRequired()
                .HasMaxLength(2000);
            builder.HasMany(x => x.AnswerSubmissions)
                    .WithOne(x => x.Answer)
                    .HasForeignKey(x => x.AnswerId);
            builder.HasOne(x => x.Question)
                    .WithMany(pt => pt.Answers)
                    .HasForeignKey(x => x.QuestionId);
        }
    }
}
