using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace RazorFrontend.Pages.Universities
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _config;

        public IndexModel(IHttpClientFactory factory, IConfiguration config)
        {
            _factory = factory;
            _config = config;
        }

        public List<UniversityDto> Universities { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _factory.CreateClient();
            var baseUrl = _config["ApiSettings:BaseUrl"];
            var endpoint = $"{baseUrl}api/university";

            var response = await client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ApiResponses<UniversityDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                Universities = result?.Data ?? new List<UniversityDto>();
            }
            else
            {
                // optional: log error or show message
                Universities = new List<UniversityDto>();
            }
        }
    }
}
