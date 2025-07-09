using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class Question : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public List<Answer> Answers { get; set; } = new List<Answer>();
        public Guid TestId { get; set; }
        public Test? Test { get; set; } = null!; // Navigation property to the Test entity
    }

    public class QuestionConfig : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.Property(x => x.Text).IsRequired(true).HasMaxLength(2000);

            builder.HasMany(x => x.Answers)
                    .WithOne(x => x.Question)
                    .HasForeignKey(x => x.QuestionId);
            builder.HasOne(x => x.Test)
                    .WithMany(pt => pt.Questions)
                    .HasForeignKey(x => x.TestId);
        }
    }
}
