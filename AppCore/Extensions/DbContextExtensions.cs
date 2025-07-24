using AppCore.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.Extensions
{
    /// <summary>
    /// Extension methods for setting up database context
    /// </summary>
    public static class DbContextExtensions
    {   
        /// <summary>
        /// Ensures that the database is created and migrated to the latest version
        /// </summary>
        public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
            }
            
            return app;
        }

    }
}