using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class Personality : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid PersonalityTypeId { get; set; }
        public PersonalityType? PersonalityType { get; set; } = null!; // Navigation property to the PersonalityType entity
        public List<TestSubmission> TestSubmissions { get; set; } = new List<TestSubmission>();
        public List<MajorPersonality> Majors { get; set; } = new List<MajorPersonality>();
    }

    public class PersonalityConfig : IEntityTypeConfiguration<Personality>
    {
        public void Configure(EntityTypeBuilder<Personality> builder)
        {
            builder.Property(x => x.Name).IsRequired(true).HasMaxLength(255);
            builder.Property(x => x.Description).IsRequired(true).HasMaxLength(2000);
            builder.HasMany(x => x.TestSubmissions)
                .WithOne(x => x.Personality)
                .HasForeignKey(x => x.TestId);
            builder.HasMany(x => x.Majors)
                .WithOne(x => x.Personality)
                .HasForeignKey(x => x.PersonalityId);
            builder.HasOne(x => x.PersonalityType)
                    .WithMany(pt => pt.Personalities)
                    .HasForeignKey(x => x.PersonalityTypeId);
        }
    }
}
