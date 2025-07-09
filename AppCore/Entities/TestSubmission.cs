using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class TestSubmission : BaseEntity
    {
        public DateTime Date { get; set; }
        public Guid TestId { get; set; }
        public Test? Test { get; set; } = null!;
        public Guid ExamineeId { get; set; }
        public UserProfile? Examinee { get; set; } = null!;
        public Guid PersonalityId { get; set; }
        public Personality? Personality { get; set; } = null!;
        public List<AnswerSubmission> Answers { get; set; } = new List<AnswerSubmission>();
    }
    public class TestSubmissionConfig : IEntityTypeConfiguration<TestSubmission>
    {
        public void Configure(EntityTypeBuilder<TestSubmission> builder)
        {
            builder.Property(u => u.Date).IsRequired().HasDefaultValue(DateTime.MinValue);
            builder.HasMany(x => x.Answers)
                    .WithOne(x => x.TestSubmission)
                    .HasForeignKey(x => x.TestSubmissionId)
                    .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Test)
                    .WithMany(pt => pt.TestSubmissions)
                    .HasForeignKey(x => x.TestId);
            builder.HasOne(x => x.Examinee)
                    .WithMany(pt => pt.TestSubmissions)
                    .HasForeignKey(x => x.ExamineeId);
            builder.HasOne(x => x.Personality)
                    .WithMany(pt => pt.TestSubmissions)
                    .HasForeignKey(x => x.PersonalityId);
        }
    }
}
