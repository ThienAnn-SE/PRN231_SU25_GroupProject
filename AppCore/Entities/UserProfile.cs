using AppCore.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppCore.Entities
{
    public class UserProfile: BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public DateTime DayOfBirth { get; set; }
        public Gender Gender { get; set; } = Gender.Other;
        public string Address { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public Guid UserAuthId { get; set; }
        public User? UserAuth { get; set; } = null!;
        public List<TestSubmission> TestSubmissions { get; set; } = new List<TestSubmission>();
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public class UserProfileConfig : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            // Required fields
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.DayOfBirth).IsRequired().HasDefaultValue(DateTime.MinValue);
            // Configure Gender as enum string
            builder.Property(u => u.Gender)
                .HasConversion<string>()
                .IsRequired();

            // Other fields
            builder.Property(u => u.Address).HasMaxLength(500);
            builder.Property(u => u.ProfilePictureUrl).HasMaxLength(1000);

            // Configure one-to-one relationship with UserAuth
            builder.HasOne(u => u.UserAuth)
                .WithOne()
                .HasForeignKey<UserProfile>(u => u.UserAuthId);
            builder.HasMany(x => x.TestSubmissions)
                    .WithOne(x => x.Examinee)
                    .HasForeignKey(x => x.ExamineeId);

        }
    }
}
