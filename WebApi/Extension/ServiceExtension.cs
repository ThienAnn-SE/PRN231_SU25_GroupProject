using ApiService.Services;
using ApiService.Services.Interfaces;

namespace ApiService.Extension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IPersonalityService, PersonalityService>();
            services.AddScoped<ITestService, TestService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<ITestSubmissionService, TestSubmissionService>();
            services.AddScoped<IUniversityService, UniversityService>();
            services.AddScoped<IMajorService, MajorService>();
            return services;
        }
    }
}
