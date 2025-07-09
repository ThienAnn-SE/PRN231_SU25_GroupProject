using AppCore.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Repositories.Extensions
{
    /// <summary>
    /// Extension methods for registering repository services in dependency injection
    /// </summary>
    public static class RepositoryServiceExtensions
    {
        /// <summary>
        /// Adds the repository layer services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddRepositories(this IServiceCollection services, string connectionString)
        {
            services.AddAppDbContext(connectionString);
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}