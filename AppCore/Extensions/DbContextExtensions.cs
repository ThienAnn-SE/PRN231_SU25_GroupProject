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
        /// Adds the ApplicationDbContext to the specified IServiceCollection
        /// </summary>
        public static IServiceCollection AddAppDbContext(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.EnableDetailedErrors();
            });
            
            // Register DbTransaction
            //services.AddTransient<IDbTransaction, DbTransaction>();
            
            return services;
        }
        
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