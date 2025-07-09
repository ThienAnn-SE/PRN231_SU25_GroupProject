using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class Test : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Question> Questions { get; set; } = new List<Question>();
        public Guid PersonalityTypeId { get; set; }
        public PersonalityType? PersonalityType { get; set; } = null!; // Navigation property to the PersonalityType entity
        public List<TestSubmission> TestSubmissions { get; set; } = new List<TestSubmission>();
    }

    public class TestConfig : IEntityTypeConfiguration<Test>
    {
        public void Configure(EntityTypeBuilder<Test> builder)
        {
            builder.Property(x => x.Title).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);

            builder.HasMany(x => x.Questions)
                .WithOne(q => q.Test)
                .HasForeignKey(q => q.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.TestSubmissions)
                .WithOne(ts => ts.Test)
                .HasForeignKey(ts => ts.TestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PersonalityType)
                .WithMany(pt => pt.Tests)
                .HasForeignKey(x => x.PersonalityTypeId);
        }
    }
}
