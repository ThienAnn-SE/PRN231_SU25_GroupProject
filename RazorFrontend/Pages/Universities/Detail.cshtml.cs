using AppCore.BaseModel;
using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace RazorFrontend.Pages.Universities
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

        public UniversityDto? University { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                string endpoint = string.Format(_config["ApiSettings:UniversityByIdEndpoint"]!, id);
                var response = await _client.GetAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = $"Failed to load university: {response.StatusCode}";
                    return Page();
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<UniversityDto>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result?.Success == true && result.Data != null)
                {
                    University = result.Data;
                }
                else
                {
                    ErrorMessage = result?.Message ?? "Unexpected error.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }

            return Page();
        }
    }
}
