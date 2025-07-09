using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class Major : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequiredSkills { get; set; } = string.Empty;
        public Guid UniversityId { get; set; }
        public University? University { get; set; } = null!; // Navigation property to the University entity
        public List<MajorPersonality> Personalities { get; set; } = new List<MajorPersonality>();
    }

    public class MajorConfig : IEntityTypeConfiguration<Major>
    {
        public void Configure(EntityTypeBuilder<Major> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000);
            builder.Property(x => x.RequiredSkills)
                .IsRequired()
                .HasMaxLength(2000);
            builder.HasMany(x => x.Personalities)
                .WithOne()
                .HasForeignKey(x => x.MajorId);
            builder.HasOne(x => x.University)
                .WithMany(pt => pt.Majors)
                .HasForeignKey(x => x.UniversityId);
        }
    }
}
