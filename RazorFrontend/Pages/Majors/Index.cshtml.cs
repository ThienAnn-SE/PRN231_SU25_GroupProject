using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using AppCore.BaseModel;

namespace RazorFrontend.Pages.Majors
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

        public List<MajorWithUniversityDto> Majors { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var client = _factory.CreateClient("ApiClient");
            var endpoint = _config["ApiSettings:MajorWithUniversityEndpoint"];

            try
            {
                var response = await client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponses<MajorWithUniversityDto>>();

                    if (apiResponse?.Success == true && apiResponse.Data != null)
                    {
                        Majors = apiResponse.Data;
                    }
                    else
                    {
                        ErrorMessage = apiResponse?.Message ?? "Unknown error while fetching majors.";
                    }
                }
                else
                {
                    ErrorMessage = "Failed to fetch majors from server.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }
    }
}
