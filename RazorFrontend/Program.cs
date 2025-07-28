using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace RazorFrontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Razor Pages services
            builder.Services.AddRazorPages();

            // Bind ApiSettings from appsettings.json
            builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

            // Add named HttpClient with BaseAddress from config
            builder.Services.AddHttpClient("ApiClient", (sp, client) =>
            {
                var config = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
                client.BaseAddress = new Uri(config.BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }

    // ApiSettings class to bind config from appsettings.json
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string LoginEndpoint { get; set; } = string.Empty;
    }
}
