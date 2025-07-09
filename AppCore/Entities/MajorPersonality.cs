using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class MajorPersonality : BaseEntity
    {
        public Guid MajorId { get; set; }
        public Major? Major { get; set; } = null!; // Navigation property to the Major entity
        public Guid PersonalityId { get; set; }
        public Personality? Personality { get; set; } = null!; // Navigation property to the Personality entity
    }

    public class MajorPersonalityConfig : IEntityTypeConfiguration<MajorPersonality>
    {
        public void Configure(EntityTypeBuilder<MajorPersonality> builder)
        {
            builder.HasOne(x => x.Major)
                    .WithMany(pt => pt.Personalities)
                    .HasForeignKey(x => x.MajorId);
            builder.HasOne(x => x.Personality)
                    .WithMany(pt => pt.Majors)
                    .HasForeignKey(x => x.PersonalityId);
        }
    }
}
