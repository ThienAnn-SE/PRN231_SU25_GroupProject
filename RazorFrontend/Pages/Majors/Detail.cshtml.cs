using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace RazorFrontend.Pages.Majors
{
    public class DetailModel : PageModel
    {
        private readonly IHttpClientFactory _factory;
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public DetailModel(IHttpClientFactory factory, HttpClient client, IConfiguration config)
        {
            _client = factory.CreateClient("ApiClient");
            _config = config;
        }

        public MajorDto? Major { get; set; }
        public List<UniversityDto> Universities { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public string GetUniversityName(Guid id)
        {
            return Universities.FirstOrDefault(u => u.Id == id)?.Name ?? "Unknown";
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                // 1. Lấy major by ID
                string endpoint = string.Format(_config["ApiSettings:MajorGetByIdEndpoint"]!, id);
                var response = await _client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Failed to load major: {response.StatusCode}";
                    return Page();
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<MajorDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success != true || result.Data == null)
                {
                    ErrorMessage = result?.Message ?? "Major not found.";
                    return Page();
                }

                Major = result.Data;

                // 2. Load University để hiện tên
                string uniListEndpoint = _config["ApiSettings:UniversityListEndpoint"] ?? "gateway/university";
                var uniRes = await _client.GetAsync(uniListEndpoint);
                if (uniRes.IsSuccessStatusCode)
                {
                    var uniJson = await uniRes.Content.ReadAsStringAsync();
                    var uniResult = JsonSerializer.Deserialize<ApiResponses<UniversityDto>>(uniJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (uniResult?.Success == true && uniResult.Data != null)
                        Universities = uniResult.Data;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading major: {ex.Message}";
            }

            return Page();
        }

    }

}
