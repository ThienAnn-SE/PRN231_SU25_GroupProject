using AppCore.Dtos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;

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

        public List<MajorDto> Majors { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            var client = _factory.CreateClient("ApiClient");
            var endpoint = _config["ApiSettings:MajorListEndpoint"];

            try
            {
                var response = await client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    Majors = await response.Content.ReadFromJsonAsync<List<MajorDto>>() ?? new();
                }
                else
                {
                    ErrorMessage = "Failed to fetch majors.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }
    }
}
