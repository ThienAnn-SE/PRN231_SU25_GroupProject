using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppCore.Dtos;
using AppCore.BaseModel;
using Microsoft.Extensions.Configuration;

namespace RazorFrontend.Pages.Personalities
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _config;

        public IndexModel(IHttpClientFactory factory, IConfiguration config)
        {
            _client = factory.CreateClient("ApiClient");
            _config = config;
        }

        public List<PersonalityDetailDto> Personalities { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                string endpoint = _config["ApiSettings:PersonalityListEndpoint"] ?? "api/personality";

                var response = await _client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponses<PersonalityDetailDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.Success == true && result.Data != null)
                    {
                        Personalities = result.Data;
                    }
                    else
                    {
                        ErrorMessage = result?.Message ?? "Unexpected error occurred.";
                    }
                }
                else
                {
                    ErrorMessage = $"API call failed: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error retrieving data: {ex.Message}";
            }
        }
    }
}
