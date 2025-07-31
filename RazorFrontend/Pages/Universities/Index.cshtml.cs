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
        private readonly HttpClient _client;

        public IndexModel(IHttpClientFactory factory, IConfiguration config)
        {
            _factory = factory;
            _config = config;
            _client = factory.CreateClient("ApiClient");
        }

        public List<UniversityDto> Universities { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                string endpoint = _config["ApiSettings:UniversityListEndpoint"] ?? "gateway/university";

                var response = await _client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponses<UniversityDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.Success == true && result.Data != null)
                    {
                        Universities = result.Data;
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

        //public async Task OnGetAsync()
        //{
        //    var client = _factory.CreateClient();
        //    var baseUrl = _config["ApiSettings:BaseUrl"];
        //    var endpoint = $"{baseUrl}api/University";

        //    var response = await client.GetAsync(endpoint);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var json = await response.Content.ReadAsStringAsync();
        //        Universities = JsonSerializer.Deserialize<List<UniversityDto>>(json, new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        }) ?? new List<UniversityDto>();
        //    }
        //}
    }
}
