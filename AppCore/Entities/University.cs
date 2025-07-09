using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class University : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Major> Majors { get; set; } = new List<Major>();
    }

    public class UniversityConfig : IEntityTypeConfiguration<University>
    {
        public void Configure(EntityTypeBuilder<University> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(x => x.Location)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(x => x.Website)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasMany(x => x.Majors)
                    .WithOne(x => x.University)
                    .HasForeignKey(x => x.UniversityId)
                     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
