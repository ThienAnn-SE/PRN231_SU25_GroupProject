using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace RazorFrontend.Pages.Majors
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _config;
        private readonly HttpClient _client;

        public IndexModel(IHttpClientFactory factory, IConfiguration config)
        {
            _factory = factory;
            _config = config;
            _client = factory.CreateClient("ApiClient");
        }

        public List<MajorDto> Majors { get; set; } = new();
        public List<UniversityDto> Universities { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Gọi danh sách Major
                string majorEndpoint = _config["ApiSettings:MajorListEndpoint"] ?? "api/major";
                var majorResponse = await _client.GetAsync(majorEndpoint);

                if (majorResponse.IsSuccessStatusCode)
                {
                    var majorJson = await majorResponse.Content.ReadAsStringAsync();
                    var majorResult = JsonSerializer.Deserialize<ApiResponses<MajorDto>>(majorJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (majorResult?.Success == true && majorResult.Data != null)
                    {
                        Majors = majorResult.Data;
                    }
                    else
                    {
                        ErrorMessage = majorResult?.Message ?? "Failed to load majors.";
                        return;
                    }
                }
                else
                {
                    ErrorMessage = $"Major API call failed: {majorResponse.StatusCode}";
                    return;
                }

                // Gọi danh sách University
                string uniEndpoint = _config["ApiSettings:UniversityListEndpoint"] ?? "api/university";
                var uniResponse = await _client.GetAsync(uniEndpoint);

                if (uniResponse.IsSuccessStatusCode)
                {
                    var uniJson = await uniResponse.Content.ReadAsStringAsync();
                    var uniResult = JsonSerializer.Deserialize<ApiResponses<UniversityDto>>(uniJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (uniResult?.Success == true && uniResult.Data != null)
                    {
                        Universities = uniResult.Data;
                    }
                    else
                    {
                        ErrorMessage = uniResult?.Message ?? "Failed to load universities.";
                    }
                }
                else
                {
                    ErrorMessage = $"University API call failed: {uniResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unexpected error: {ex.Message}";
            }
        }
    }
}
