using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class PersonalityType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Personality> Personalities { get; set; } = new List<Personality>();
        public List<Test> Tests { get; set; } = new List<Test>();

    }

    public class PersonalityTypeConfig : IEntityTypeConfiguration<PersonalityType>
    {
        public void Configure(EntityTypeBuilder<PersonalityType> builder)
        {
            builder.Property(x => x.Name).IsRequired(true).HasMaxLength(255);
            builder.Property(x => x.Description).IsRequired(true).HasMaxLength(2000);
            builder.HasMany(x => x.Personalities)
                    .WithOne(x => x.PersonalityType)
                    .HasForeignKey(x => x.PersonalityTypeId);
            builder.HasMany(x => x.Tests)
                    .WithOne(x => x.PersonalityType)
                    .HasForeignKey(x => x.PersonalityTypeId);
        }
    }
}
