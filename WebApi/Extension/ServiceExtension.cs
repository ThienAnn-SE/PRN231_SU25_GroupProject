using WebApi.Services;

namespace WebApi.Extension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPersonalityService, PersonalityService>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<ITestSubmissionService, TestSubmissionService>();
            services.AddScoped<IUniversityService, UniversityService>();
            return services;
        }
    }
}
