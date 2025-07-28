using AppCore.BaseModel;
using AppCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AppCore.Data
{
    /// <summary>
    /// Application database context for Entity Framework Core
    /// </summary>
    public class ApplicationDbContext : DbContext
    {

        /// <summary>
        /// Refresh tokens used for authentication
        /// </summary>
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        
        /// <summary>
        /// External login providers linked to user accounts
        /// </summary>
        public DbSet<ExternalLogin> ExternalLogins { get; set; } = null!;

        /// <summary>
        /// Users registered in the application
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// User profile details associated with users
        /// </summary>
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;

        /// <summary>
        /// Tests available in the system
        /// </summary>
        public DbSet<Test> Tests { get; set; } = null!;

        /// <summary>
        /// Questions belonging to tests
        /// </summary>
        public DbSet<Question> Questions { get; set; } = null!;

        /// <summary>
        /// Possible answers for questions
        /// </summary>
        public DbSet<Answer> Answers { get; set; } = null!;

        /// <summary>
        /// Submissions of answers by users for test questions
        /// </summary>
        public DbSet<AnswerSubmission> AnswerSubmissions { get; set; } = null!;

        /// <summary>
        /// Academic majors offered by universities
        /// </summary>
        public DbSet<Major> Majors { get; set; } = null!;

        /// <summary>
        /// Relationship between majors and associated personalities
        /// </summary>
        public DbSet<MajorPersonality> MajorPersonalities { get; set; } = null!;

        /// <summary>
        /// Personality profiles used for test results and recommendations
        /// </summary>
        public DbSet<Personality> Personalities { get; set; } = null!;

        /// <summary>
        /// Types or categories of personalities
        /// </summary>
        public DbSet<PersonalityType> PersonalityTypes { get; set; } = null!;

        /// <summary>
        /// Submissions of completed tests by users
        /// </summary>
        public DbSet<TestSubmission> TestSubmissions { get; set; } = null!;

        /// <summary>
        /// Universities participating in the system
        /// </summary>
        public DbSet<University> Universities { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext() { }

        private static string GetConnectionString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            return configuration.GetConnectionString("DefaultConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RefreshTokenConfig());
            modelBuilder.ApplyConfiguration(new ExternalLoginConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new UserProfileConfig());
            modelBuilder.ApplyConfiguration(new TestConfig());
            modelBuilder.ApplyConfiguration(new QuestionConfig());
            modelBuilder.ApplyConfiguration(new AnswerSubmissionConfig());
            modelBuilder.ApplyConfiguration(new AnswerConfig());
            modelBuilder.ApplyConfiguration(new MajorConfig());
            modelBuilder.ApplyConfiguration(new MajorPersonalityConfig());
            modelBuilder.ApplyConfiguration(new PersonalityConfig());
            modelBuilder.ApplyConfiguration(new PersonalityTypeConfig());
            modelBuilder.ApplyConfiguration(new TestSubmissionConfig());
            modelBuilder.ApplyConfiguration(new UniversityConfig());

        }

        /// <summary>
        /// Automatically sets audit properties on entities before saving changes
        /// </summary>
        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }
        
        /// <summary>
        /// Automatically sets audit properties on entities before saving changes asynchronously
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }
        
        /// <summary>
        /// Updates audit fields (CreatedAt, UpdatedAt) on entities
        /// </summary>
        private void UpdateAuditFields()
        {
            var now = DateTime.UtcNow;
            
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.UpdatedAt = now;
                        break;
                    
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        
                        // Don't modify CreatedAt on updates
                        entry.Property(p => p.CreatedAt).IsModified = false;
                        
                        // Don't modify CreatorId on updates
                        entry.Property(p => p.CreatorId).IsModified = false;
                        break;
                }
            }
        }
    }
}