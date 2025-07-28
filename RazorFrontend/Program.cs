using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace RazorFrontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Nạp thêm file settings.json
            builder.Configuration.AddJsonFile("settings.json", optional: true, reloadOnChange: true);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
            builder.Services.AddHttpClient("ApiClient", (sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var baseUrl = config["ApiSettings:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl!);
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            var app = builder.Build();

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

    public class ApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string LoginEndpoint { get; set; } = string.Empty;
    }
}
