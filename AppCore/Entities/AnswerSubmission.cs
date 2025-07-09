using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class AnswerSubmission : BaseEntity
    {
        public Guid AnswerId { get; set; }
        public Answer? Answer { get; set; } = null!;
        public Guid TestSubmissionId { get; set; }
        public TestSubmission? TestSubmission { get; set; } = null!;
    }

    public class AnswerSubmissionConfig : IEntityTypeConfiguration<AnswerSubmission>
    {
        public void Configure(EntityTypeBuilder<AnswerSubmission> builder)
        {
            builder.HasOne(x => x.Answer)
                    .WithMany(pt => pt.AnswerSubmissions)
                    .HasForeignKey(x => x.AnswerId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TestSubmission)
                    .WithMany(pt => pt.Answers)
                    .HasForeignKey(x => x.TestSubmissionId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
