using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AppCore.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private const string DefaultConnectionString = "Server=localhost;Database=PRN231_SU25_SPES;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Find the WebApi directory, regardless of current directory
            string currentDir = Directory.GetCurrentDirectory();
            string solutionDir = FindSolutionDirectory(currentDir);
            string webApiPath = Path.Combine(solutionDir, "WebApi");
            
            // Check if directory exists
            if (!Directory.Exists(webApiPath))
            {
                // Fallback to a hardcoded path if needed
                string fallbackPath = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "WebApi"));
                if (Directory.Exists(fallbackPath))
                {
                    webApiPath = fallbackPath;
                }
                else
                {
                    // Last resort - use a connection string directly
                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseSqlServer(DefaultConnectionString);
                    return new ApplicationDbContext(optionsBuilder.Options);
                }
            }
            
            string configPath = Path.Combine(webApiPath, "appsettings.json");
            
            if (!File.Exists(configPath))
            {
                // If config doesn't exist, use a hardcoded connection string
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(DefaultConnectionString);
                return new ApplicationDbContext(optionsBuilder.Options);
            }
            
            // Try to load configuration
            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile(configPath, optional: false)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection") 
                    ?? DefaultConnectionString;

                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                return new ApplicationDbContext(optionsBuilder.Options);
            }
            catch (Exception)
            {
                // Last resort, use hardcoded connection
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(DefaultConnectionString);
                return new ApplicationDbContext(optionsBuilder.Options);
            }
        }
        
        private string FindSolutionDirectory(string startDir)
        {
            // Try to find the solution directory by looking for a .sln file
            DirectoryInfo dir = new DirectoryInfo(startDir);
            
            while (dir != null)
            {
                if (dir.GetFiles("*.sln").Length > 0)
                {
                    return dir.FullName;
                }
                
                dir = dir.Parent;
            }
            
            // If not found, return the starting directory
            return startDir;
        }
    }
}